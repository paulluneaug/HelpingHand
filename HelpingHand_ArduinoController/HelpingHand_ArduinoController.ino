
#include <SoftwareSerial.h>
#include <SerialCommand.h>
#include <SimpleRotary.h>

#pragma region Type defs

const int MAX_QUEUE_SIZE = 64;

typedef struct {
  byte items[MAX_QUEUE_SIZE];
  int front;
  int rear;
  int count;
} Queue;

typedef struct {
  byte rotation;
  byte click;
} RotaryState;

#pragma endregion

#pragma region Queue Functions

////////////////
// QUEUE
////////////////

// Function to initialize the queue
void InitializeQueue(Queue* q) {
  q->front = -1;
  q->rear = 0;
  q->count = 0;
}

// Function to check if the queue is empty
bool IsEmpty(Queue* q) {
  int frontMod = (q->front + MAX_QUEUE_SIZE) % MAX_QUEUE_SIZE;
  int rearMod = (q->rear + MAX_QUEUE_SIZE) % MAX_QUEUE_SIZE;
  return frontMod + 1 == rearMod;
}

// Function to check if the queue is full
bool IsFull(Queue* q) {
  int frontMod = (q->front + MAX_QUEUE_SIZE) % MAX_QUEUE_SIZE;
  int rearMod = (q->rear + MAX_QUEUE_SIZE) % MAX_QUEUE_SIZE;
  return frontMod == rearMod + 1;
}

// Function to add an element to the queue (Enqueue
// operation)
void Enqueue(Queue* q, byte value) {
  if (IsFull(q)) {
    return;
  }
  q->items[q->rear] = value;
  q->rear = (q->rear + 1) % MAX_QUEUE_SIZE;
  q->count++;
}

// Function to remove an element from the queue (Dequeue
// operation)
void Dequeue(Queue* q) {
  if (IsEmpty(q)) {
    return;
  }
  q->front = (q->front + 1) % MAX_QUEUE_SIZE;
  q->count--;
}

// Function to get the element at the front of the queue
// (Peek operation)
byte Peek(Queue* q) {
  if (IsEmpty(q)) {
    return 0;  // return some default value or handle
               // error differently
  }
  return q->items[(q->front + 1) % MAX_QUEUE_SIZE];
}
////////////////
// END QUEUE
////////////////

#pragma endregion

#pragma region RotaryState Functions

void InitializeRotaryState(RotaryState* rotaryState) {
  rotaryState->rotation = 0;
  rotaryState->click = 0;
  return;
}

bool UpdateRotaryState(RotaryState* rotaryState, byte newRotation, byte newClick)
{
  bool changed = false;
  if (newRotation != rotaryState->rotation)
  {
    rotaryState->rotation = newRotation;
    changed = true;
  }
  if (newClick != rotaryState->click)
  {
    rotaryState->click = newClick;
    changed = true;
  }
  return changed;
}
#pragma endregion


// Pins
const int ROTARY_CLK_PIN = 4;
const int ROTARY_DT_PIN = 3;
const int ROTARY_SW_PIN = 2;
const int FADER_PIN = 5;

// Headers
const byte ROTARY_STATE_HEADER = 10;
const byte DMX_COMMAND_HEADER = 20;
const byte ERROR_HEADER = 245;
const byte DEBUG_MODE_HEADER = 100; // d


const int BUFFER_SIZE = 32;

bool m_debug;

int m_wroteBytes = 0;
byte m_readBuffer[BUFFER_SIZE];
byte m_writeBuffer[BUFFER_SIZE];


SimpleRotary m_rotary(ROTARY_CLK_PIN, ROTARY_DT_PIN, ROTARY_SW_PIN);
RotaryState m_rotaryState;


Queue m_recieveQueue;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);

  m_debug = true;

  InitializeQueue(&m_recieveQueue);
  InitRotary();
  //pinMode(CLICK_PIN, INPUT);
  //pinMode(DATA_PIN, INPUT);
  //pinMode(SW_PIN, INPUT);

}

void InitRotary()
{
  InitializeRotaryState(&m_rotaryState);
}

void loop() 
{ 
  //ReadSerial();

  m_wroteBytes = 0;
  UpdateRotary();
  SendDataIfNeeded();
  delay(10);
}

void ReadSerial()
{
  if (Serial.available()) {
    int readBytes = Serial.readBytes(m_readBuffer, BUFFER_SIZE);
    //SendError(readBytes);

    DispatchRecievedMessage(readBytes);

    Serial.flush();
  }
}

void DispatchRecievedMessage(int readBytes) {
  for (int i = 0; i < readBytes; i++) {
    Enqueue(&m_recieveQueue, m_readBuffer[i]);
  }

  bool recievedEnoughDatas = true;
  while (m_recieveQueue.count > 0 && recievedEnoughDatas) {
    byte queueHead = Peek(&m_recieveQueue);
    switch (queueHead) {
      case DEBUG_MODE_HEADER:
        recievedEnoughDatas &= TryProcessDebugCommand(&m_recieveQueue);
        break;

      default:  // The header is discarded if unknown
        Dequeue(&m_recieveQueue);
        SendError(queueHead);
        break;
    }
  }
}

bool TryProcessDebugCommand(Queue* recievedDatas)
{
  Dequeue(recievedDatas);  // Dequeue the header
  m_debug = !m_debug;
  return true;
}

void SendDataIfNeeded()
{
  if (m_wroteBytes != 0)
  {
    if (m_debug)
    {
      Serial.println("");
    }
    else
    {
      Serial.write(m_writeBuffer, m_wroteBytes);
    }
  }
}

void UpdateRotary()
{
  byte rotation = m_rotary.rotate();
  byte click = digitalRead(ROTARY_SW_PIN) == HIGH ? 0 : 1;

  if (UpdateRotaryState(&m_rotaryState, rotation, click))
  {
    if (m_debug)
    {
      Serial.print(rotation);
      Serial.print(" ");
      Serial.print(click);
      Serial.print(" ");
    }
    else
    {
      m_writeBuffer[m_wroteBytes++] = ROTARY_STATE_HEADER;
      m_writeBuffer[m_wroteBytes++] = rotation;
      m_writeBuffer[m_wroteBytes++] = click;
    }
  }

}


void SendError(byte errorCode) 
{
  m_writeBuffer[m_wroteBytes++] = ERROR_HEADER;
  m_writeBuffer[m_wroteBytes++] = errorCode;
}
