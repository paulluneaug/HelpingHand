#include <SimpleRotary.h>
#include <Joystick.h>
#include <Queue.h>

#pragma region Type defs

const int MAX_QUEUE_SIZE = 256;


typedef struct 
{
  byte rotation;
} RotaryState;

#pragma endregion

#pragma region RotaryState Functions

void InitializeRotaryState(RotaryState* rotaryState) {
  rotaryState->rotation = 0;
  return;
}

bool UpdateRotaryState(RotaryState* rotaryState, byte newRotation)
{
  bool changed = false;
  if (newRotation != rotaryState->rotation)
  {
    rotaryState->rotation = newRotation;
    changed = true;
  }
  return changed;
}
#pragma endregion


// Pins
const int ROTARY_CLK_PIN = 4;
const int ROTARY_DT_PIN = 3;
const int FADER_X_PIN = 6;
const int FADER_Y_PIN = 7;

const int INDICATORS_PINS [32] = 
{
  2,
  3,
  4,
  5,
  6,
  7,
  8,
  9,
  10,
  11,
  12,
  13,
  14,
  15,
  -1,
  -1,
  -1,
  -1,
  -1,
  -1,
  -1,
  -1,
  -1,
  -1,
  -1,
  -1,
  -1,
  -1,
  -1,
  -1,
  -1,
  -1,
};

// Headers
const byte INDICATOR_STATE_HEADER = 10;
const byte ERROR_HEADER = 245;

const int ROTARY_LEFT_BUTTON_INDEX = 0; 
const int ROTARY_RIGHT_BUTTON_INDEX = 1;


const int BUFFER_SIZE = 64;

int m_wroteBytes = 0;
byte m_readBuffer[BUFFER_SIZE];
byte m_writeBuffer[BUFFER_SIZE];


SimpleRotary m_rotary(ROTARY_CLK_PIN, ROTARY_DT_PIN, 40);
RotaryState m_rotaryState{};

Joystick_ m_joystick {};

Queue<byte, MAX_QUEUE_SIZE> m_recieveQueue{};

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);

  for (int indicator = 0; indicator < 32; ++indicator)
  {
    int pin = INDICATORS_PINS[indicator];
    if (pin == -1)
    {
      continue;
    }
    
    pinMode(pin, OUTPUT);
  }

  InitJoystick();
  InitRotary();

}

void InitRotary()
{
  InitializeRotaryState(&m_rotaryState);
}

void InitJoystick()
{
  m_joystick.begin();
}

void loop() 
{ 
  m_wroteBytes = 0;

  ReadSerial();

  UpdateRotary();

  m_joystick.setXAxis(analogRead(FADER_X_PIN));
  m_joystick.setYAxis(analogRead(FADER_Y_PIN));

  m_joystick.sendState();

  SendDataIfNeeded();
  delay(10);
}

void ReadSerial()
{
  if (Serial.available()) {
    int readBytes = Serial.readBytes(m_readBuffer, BUFFER_SIZE);
    SendError(readBytes);

    DispatchRecievedMessage(readBytes);

    Serial.flush();
    m_dataRecieved = true;
  }
}

void DispatchRecievedMessage(int readBytes) {
  for (int i = 0; i < readBytes; i++) 
  {
    m_recieveQueue.Enqueue(m_readBuffer[i]);
  }

  bool recievedEnoughDatas = true;
  while (m_recieveQueue.Count() > 0 && recievedEnoughDatas) {
    byte queueHead = m_recieveQueue.Peek();
    switch (queueHead) 
    {
      case INDICATOR_STATE_HEADER:
        recievedEnoughDatas &= TryProcessIndicatorStateCommand();
        break;


      default:  // The header is discarded if unknown
        m_recieveQueue.Dequeue();
        SendError(queueHead);
        break;
    }
  }
}

bool TryProcessIndicatorStateCommand()
{  
  if (m_recieveQueue.Count() < 5)
  {
    return false;  // Should wait for more datas to arrive
  }
  m_recieveQueue.Dequeue();  // Dequeue the header

  int indicatorStatePart0 = m_recieveQueue.Peek();
  m_recieveQueue.Dequeue();
  int indicatorStatePart1 = m_recieveQueue.Peek();
  m_recieveQueue.Dequeue();
  int indicatorStatePart2 = m_recieveQueue.Peek();
  m_recieveQueue.Dequeue();
  int indicatorStatePart3 = m_recieveQueue.Peek();
  m_recieveQueue.Dequeue();
  
  int indicatorsState = 
    indicatorStatePart0 << 0 |
    indicatorStatePart1 << 8 |
    indicatorStatePart2 << 16 |
    indicatorStatePart3 << 24;

  UpdateIndicators(indicatorsState);
}

void SendDataIfNeeded()
{
  if (m_wroteBytes != 0)
  {
    Serial.write(m_writeBuffer, m_wroteBytes);
  }
}

void UpdateRotary()
{
  byte rotation = m_rotary.rotate();

  if (UpdateRotaryState(&m_rotaryState, rotation))
  {
    UpdateRotaryDirectionButtons(rotation);
  }
}

void UpdateRotaryDirectionButtons(byte rotationCode)
{
  switch(rotationCode)
  {
    case 0:
      m_joystick.setButton(ROTARY_LEFT_BUTTON_INDEX, 0);
      m_joystick.setButton(ROTARY_RIGHT_BUTTON_INDEX, 0);
      break;
    case 1:
      m_joystick.setButton(ROTARY_LEFT_BUTTON_INDEX, 0);
      m_joystick.setButton(ROTARY_RIGHT_BUTTON_INDEX, 1);
      break;
    case 2:
      m_joystick.setButton(ROTARY_LEFT_BUTTON_INDEX, 1);
      m_joystick.setButton(ROTARY_RIGHT_BUTTON_INDEX, 0);
      break;
    default:
      break;
  };
}

void UpdateIndicators(int indicatorsState)
{
  for (int bitIndex = 0; bitIndex < 32; ++bitIndex)
  {
    int pin = INDICATORS_PINS[bitIndex];
    bool on = (indicatorsState & (1 << bitIndex)) != 0;

    m_joystick.setButton(bitIndex, on ? HIGH : LOW);
    if (pin == -1)
    {
      continue;
    }
    
    digitalWrite(pin, on ? HIGH : LOW);
  }
}

void SendError(byte errorCode) 
{
  m_writeBuffer[m_wroteBytes++] = ERROR_HEADER;
  m_writeBuffer[m_wroteBytes++] = errorCode;
}
