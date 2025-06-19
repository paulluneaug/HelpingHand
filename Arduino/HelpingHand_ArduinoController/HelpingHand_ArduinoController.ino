#include <Joystick.h>

// Utils
#include <Queue.h>
#include <MuxAdress.h>

// Glossaries
#include <PinGlossary.h>
#include <MuxAdressGlossary.h>
#include <HeaderGlossary.h>
#include <ButtonGlossary.h>

// Controllers
#include <Fader.h>
#include <RotaryController.h>
#include <ButtonController.h>
#include <SimonController.h>


// Communication
const int MAX_QUEUE_SIZE = 256;
const int BUFFER_SIZE = 64;

int m_wroteBytes = 0;
byte m_readBuffer[BUFFER_SIZE];
byte m_writeBuffer[BUFFER_SIZE];

Queue<byte, MAX_QUEUE_SIZE> m_recieveQueue{};
// ~Communication

Joystick_ m_joystick {};


#pragma region Multiplexers
ReadMux m_mux0(PinGlossary::MUX0_A0, PinGlossary::MUX0_A1, PinGlossary::MUX0_A2, PinGlossary::MUX0_A3, PinGlossary::MUX0_SIG);
ReadMux m_mux1(PinGlossary::MUX1_A0, PinGlossary::MUX1_A1, PinGlossary::MUX1_A2, PinGlossary::MUX1_A3, PinGlossary::MUX1_SIG);
ReadMux m_mux2(PinGlossary::MUX2_A0, PinGlossary::MUX2_A1, PinGlossary::MUX2_A2, PinGlossary::MUX2_A3, PinGlossary::MUX2_SIG);

ReadMux* m_multiplexers[] = 
{
  &m_mux0,
  &m_mux1,
  &m_mux2
};
#pragma endregion

#pragma region Input Controllers

Fader m_motorizedFader(PinGlossary::M_FADER, PinGlossary::H_BRIDGE_IN0, PinGlossary::H_BRIDGE_IN1, PinGlossary::H_BRIDGE_ENA);

#pragma region Rotaries
RotaryController m_musicRotary(&m_joystick, PinGlossary::MUSIC_ROTARY_CLK, PinGlossary::MUSIC_ROTARY_DT, ButtonGlossary::MUSIC_ROTARY_LEFT, ButtonGlossary::MUSIC_ROTARY_RIGHT);
RotaryController m_selectRotary(&m_joystick, PinGlossary::SELECT_ROTARY_CLK, PinGlossary::SELECT_ROTARY_DT, ButtonGlossary::SELECT_ROTARY_LEFT, ButtonGlossary::SELECT_ROTARY_RIGHT);
RotaryController m_dropperRotary(&m_joystick, PinGlossary::DROPPER_ROTARY_CLK, PinGlossary::DROPPER_ROTARY_DT, ButtonGlossary::DROPPER_ROTARY_LEFT, ButtonGlossary::DROPPER_ROTARY_RIGHT);

RotaryController* m_rotaryControllers[] = 
{
  &m_musicRotary,
  &m_selectRotary,
  &m_dropperRotary,
};
#pragma endregion

#pragma region Mux 0
MuxAdress m_spotIntensityPot(&m_mux0, MuxAdressGlossary::MUX0_SPOT_INTENSITY_POT);

ButtonController m_binaryToggle_0(&m_mux0, MuxAdressGlossary::MUX0_BINARY_TOGGLE_0, &m_joystick, ButtonGlossary::BINARY_TOGGLE_0);
ButtonController m_binaryToggle_1(&m_mux0, MuxAdressGlossary::MUX0_BINARY_TOGGLE_1, &m_joystick, ButtonGlossary::BINARY_TOGGLE_1);
ButtonController m_binaryBtn(&m_mux0, MuxAdressGlossary::MUX0_BINARY_BTN, &m_joystick, ButtonGlossary::BINARY_BTN);

ButtonController m_toggleTrio_0(&m_mux0, MuxAdressGlossary::MUX0_TRIO_TOGGLE_0, &m_joystick, ButtonGlossary::TOGGLE_TRIO_0);
ButtonController m_toggleTrio_1(&m_mux0, MuxAdressGlossary::MUX0_TRIO_TOGGLE_1, &m_joystick, ButtonGlossary::TOGGLE_TRIO_1);
ButtonController m_toggleTrio_2(&m_mux0, MuxAdressGlossary::MUX0_TRIO_TOGGLE_2, &m_joystick, ButtonGlossary::TOGGLE_TRIO_2);

ButtonController m_spotSceneToggle(&m_mux0, MuxAdressGlossary::MUX0_SPOT_SCENE_TOGGLE, &m_joystick, ButtonGlossary::SPOT_SCENE_TOGGLE);
ButtonController m_spotAutofollowToggle(&m_mux0, MuxAdressGlossary::MUX0_SPOT_AUTOFOLLOW_TOGGLE, &m_joystick, ButtonGlossary::SPOT_AUTOFOLLOW_TOGGLE);
ButtonController m_spotPuppetToggle(&m_mux0, MuxAdressGlossary::MUX0_SPOT_PUPET_BTN, &m_joystick, ButtonGlossary::SPOT_PUPET_BTN);

ButtonController m_pauseBtn(&m_mux0, MuxAdressGlossary::MUX0_PAUSE_BTN, &m_joystick, ButtonGlossary::PAUSE);
ButtonController m_validateBtn(&m_mux0, MuxAdressGlossary::MUX0_VALIDATE_BTN, &m_joystick, ButtonGlossary::VALIDATE);

ButtonController m_dropperBtn(&m_mux0, MuxAdressGlossary::MUX0_DROPER_BTN, &m_joystick, ButtonGlossary::DROPPER_BTN);

#pragma endregion

#pragma region Mux 1
ButtonController m_btnSimonGreen(&m_mux1, MuxAdressGlossary::MUX1_SIMON_GREEN, &m_joystick, ButtonGlossary::SIMON_GREEN_BTN);
ButtonController m_btnSimonRed(&m_mux1, MuxAdressGlossary::MUX1_SIMON_RED, &m_joystick, ButtonGlossary::SIMON_RED_BTN);
ButtonController m_btnSimonWhite(&m_mux1, MuxAdressGlossary::MUX1_SIMON_WHITE, &m_joystick, ButtonGlossary::SIMON_WHITE_BTN);
ButtonController m_btnSimonBlue(&m_mux1, MuxAdressGlossary::MUX1_SIMON_BLUE, &m_joystick, ButtonGlossary::SIMON_BLUE_BTN);

MuxAdress m_btnJoystickNorth(&m_mux1, MuxAdressGlossary::MUX1_JOYSTICK_0);
MuxAdress m_btnJoystickWest(&m_mux1, MuxAdressGlossary::MUX1_JOYSTICK_1);
MuxAdress m_btnJoystickSouth(&m_mux1, MuxAdressGlossary::MUX1_JOYSTICK_2);
MuxAdress m_btnJoystickEast(&m_mux1, MuxAdressGlossary::MUX1_JOYSTICK_3);
#pragma endregion

#pragma region Mux 2
ButtonController m_btnSFX_0(&m_mux2, MuxAdressGlossary::MUX2_SFX_0, &m_joystick, ButtonGlossary::SFX_0);
ButtonController m_btnSFX_1(&m_mux2, MuxAdressGlossary::MUX2_SFX_1, &m_joystick, ButtonGlossary::SFX_1);
ButtonController m_btnSFX_2(&m_mux2, MuxAdressGlossary::MUX2_SFX_2, &m_joystick, ButtonGlossary::SFX_2);
ButtonController m_btnSFX_3(&m_mux2, MuxAdressGlossary::MUX2_SFX_3, &m_joystick, ButtonGlossary::SFX_3);
ButtonController m_btnSFX_4(&m_mux2, MuxAdressGlossary::MUX2_SFX_4, &m_joystick, ButtonGlossary::SFX_4);
ButtonController m_btnSFX_5(&m_mux2, MuxAdressGlossary::MUX2_SFX_5, &m_joystick, ButtonGlossary::SFX_5);

ButtonController m_btnStartMusic(&m_mux2, MuxAdressGlossary::MUX2_START_MUSIC_BTN, &m_joystick, ButtonGlossary::START_MUSIC);

ButtonController m_btnFlamethrower(&m_mux2, MuxAdressGlossary::MUX2_FLAMETHROWER_BTN, &m_joystick, ButtonGlossary::FLAMETHROWER_BTN);
ButtonController m_flamethrowerToggle_0(&m_mux2, MuxAdressGlossary::MUX2_FLAMETHROWER_TOGGLE_0, &m_joystick, ButtonGlossary::FLAMETHROWER_TOGGLE_0);
ButtonController m_flamethrowerToggle_1(&m_mux2, MuxAdressGlossary::MUX2_FLAMETHROWER_TOGGLE_1, &m_joystick, ButtonGlossary::FLAMETHROWER_TOGGLE_1);

MuxAdress m_flamethrowerPot(&m_mux2, MuxAdressGlossary::MUX2_FLAMETHROWER_POT);
MuxAdress m_dayNightPot(&m_mux2, MuxAdressGlossary::MUX2_DAY_NIGHT_POT);
MuxAdress m_propsFader_0(&m_mux2, MuxAdressGlossary::MUX2_PROPS_FADER_0);
MuxAdress m_propsFader_1(&m_mux2, MuxAdressGlossary::MUX2_PROPS_FADER_1);
MuxAdress m_propsSelectorFader(&m_mux2, MuxAdressGlossary::MUX2_PROPS_SELECTOR_FADER);
#pragma endregion

ButtonController* m_buttonControllers[] = 
{
  &m_binaryToggle_0,
  &m_binaryToggle_1,
  &m_binaryBtn,

  &m_toggleTrio_0,
  &m_toggleTrio_1,
  &m_toggleTrio_2,

  &m_spotSceneToggle,
  &m_spotAutofollowToggle,
  &m_spotPuppetToggle,

  &m_pauseBtn,
  &m_validateBtn,

  &m_dropperBtn,

  &m_btnSimonGreen,
  &m_btnSimonRed,
  &m_btnSimonWhite,
  &m_btnSimonBlue,

  &m_btnSFX_0,
  &m_btnSFX_1,
  &m_btnSFX_2,
  &m_btnSFX_3,
  &m_btnSFX_4,
  &m_btnSFX_5,

  &m_btnStartMusic,

  &m_btnFlamethrower,
  &m_btnStartMusic,
  &m_btnStartMusic
};

#pragma endregion


// Simon
SimonController m_simon(2, 3, 4, 5, 1.0f, 0.5f);
byte m_simonSequence[16];

// Game loop
unsigned long m_frameStart;
unsigned long m_frameEnd;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);

  m_frameStart = 0;
  m_frameEnd = 0;

  m_joystick.begin();

}

void loop() 
{ 
  float deltaTime = (m_frameEnd - m_frameStart) / 1000.0f;
  m_frameStart = millis();

  m_wroteBytes = 0;

  ReadSerial();

  m_motorizedFader.Update(deltaTime);

  UpdateRotaries();
  UpdateButtons();
  UpdateAxis();
  UpdateJoystick();

  m_simon.Update(deltaTime);

  m_joystick.sendState();

  SendDataIfNeeded();

  delay(10);

  m_frameEnd = millis();
}

void ReadSerial()
{
  if (Serial.available()) {
    int readBytes = Serial.readBytes(m_readBuffer, BUFFER_SIZE);

    DispatchRecievedMessage(readBytes);

    Serial.flush();
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
      case HeaderGlossary::INDICATOR_STATE_HEADER:
        recievedEnoughDatas &= TryProcessIndicatorStateCommand();
        break;

      case HeaderGlossary::SIMON_SEQUENCE_HEADER:
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

bool TryProcessSimonSequenceCommand()
{  
  if (m_recieveQueue.Count() < 3)
  {
    return false;  // Should wait for more datas to arrive
  }

  byte sequenceLength = m_recieveQueue.At(1); // Position of the sequenceLength in the command
  if (m_recieveQueue.Count() < sequenceLength + 2)
  {
    return false; // Should wait for more datas to arrive
  }

  m_recieveQueue.Dequeue();  // Dequeue the header
  m_recieveQueue.Dequeue();  // Dequeue the Sequence length


  byte currentByte = m_recieveQueue.Peek();
  m_recieveQueue.Dequeue();
  for (int iElement = 0; iElement < sequenceLength; ++iElement)
  {
    int offset = iElement % 4;
    m_simonSequence[iElement] = (currentByte >> offset * 2) & 0b11;
    if (offset == 3)
    {
      byte currentByte = m_recieveQueue.Peek();
      m_recieveQueue.Dequeue();
    }
  }

  m_simon.StartSequence(m_simonSequence, sequenceLength);
}

void SendDataIfNeeded()
{
  if (m_wroteBytes != 0)
  {
    Serial.write(m_writeBuffer, m_wroteBytes);
  }
}

void UpdateRotaries()
{
  for (RotaryController* rotary : m_rotaryControllers)
  {
    rotary->Update();
  }
}

void UpdateButtons()
{
  for (ButtonController* button : m_buttonControllers)
  {
    button->Update();
  }
}

void UpdateAxis()
{
  m_joystick.setXAxis(m_flamethrowerPot.ReadAnalog());
  m_joystick.setYAxis(m_dayNightPot.ReadAnalog());
  m_joystick.setZAxis(m_propsFader_0.ReadAnalog());

  m_joystick.setRxAxis(m_propsFader_1.ReadAnalog());
  m_joystick.setRyAxis(m_propsSelectorFader.ReadAnalog());
  m_joystick.setRzAxis(m_motorizedFader.ReadValue());

  m_joystick.setRudder(m_spotIntensityPot.ReadAnalog());
}

void UpdateJoystick()
{
  bool north = m_btnJoystickNorth.ReadDigital();
  bool east = m_btnJoystickEast.ReadDigital();
  bool south = m_btnJoystickSouth.ReadDigital();
  bool west = m_btnJoystickWest.ReadDigital();

  int angle = -1;

  if (north) 
  { 
    if (east){ angle = 45; }
    else if (west) { angle = 315; }
    else{ angle = 0; }
  }
  else if (south)
  {
    if (east){ angle = 135; }
    else if (west) { angle = 225; }
    else{ angle = 180; }
  }
  else if (east)
  {
    angle = 90;
  }
  else if (west)
  {
    angle = 270;
  }
  m_joystick.setHatSwitch(0, angle);
}

void UpdateIndicators(int indicatorsState)
{
  for (int bitIndex = 0; bitIndex < 32; ++bitIndex)
  {
    //int pin = INDICATORS_PINS[bitIndex];
    //bool on = (indicatorsState & (1 << bitIndex)) != 0;

    //if (pin == -1)
    //{
    //  continue;
    //}
    //
    //digitalWrite(pin, on ? HIGH : LOW);
  }
}

void SendError(byte errorCode) 
{
  m_writeBuffer[m_wroteBytes++] = HeaderGlossary::ERROR_HEADER;
  m_writeBuffer[m_wroteBytes++] = errorCode;
}
