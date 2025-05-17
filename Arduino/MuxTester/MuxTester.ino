
#include <Joystick.h>
#include <ReadMux.h>
#include <WriteMux.h>

// Pins
const int READ_MUX_ADRESS_0 = 22;
const int READ_MUX_ADRESS_1 = 23;
const int READ_MUX_ADRESS_2 = 24;
const int READ_MUX_ADRESS_3 = 25;
const int READ_MUX_SIGNAL = 2;

const int WRITE_MUX_ADRESS_0 = 50 - 2;
const int WRITE_MUX_ADRESS_1 = 51 - 2;
const int WRITE_MUX_ADRESS_2 = 52 - 2;
const int WRITE_MUX_ADRESS_3 = 53 - 2;
const int WRITE_MUX_SIGNAL = 2;

const int PIN_41 = 41;
const int PIN_42 = 42;
const int PIN_43 = 43;

Joystick_ m_joystick;

ReadMux m_readMux(
  READ_MUX_ADRESS_0, 
  READ_MUX_ADRESS_1, 
  READ_MUX_ADRESS_2, 
  READ_MUX_ADRESS_3, 
  READ_MUX_SIGNAL);

WriteMux m_writeMux(
  WRITE_MUX_ADRESS_0, 
  WRITE_MUX_ADRESS_1, 
  WRITE_MUX_ADRESS_2, 
  WRITE_MUX_ADRESS_3, 
  WRITE_MUX_SIGNAL);

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);

  pinMode(53, INPUT);

  pinMode(PIN_41, OUTPUT);
  pinMode(PIN_42, OUTPUT);
  pinMode(PIN_43, OUTPUT);

  InitJoystick();
  InitMux();

}

void InitJoystick()
{
  m_joystick.begin();
}

void InitMux()
{
  m_readMux.Init();
  m_writeMux.Init();
}

void loop() 
{
  for (byte channel = 0; channel < 16; ++channel)
  {
    Serial.print(m_readMux.ReadChannelValue(channel));
    Serial.print(" ");

    m_joystick.setButton(channel, m_readMux.ReadChannelValue(channel) != 0);
    m_writeMux.WriteChannelValue(channel, (channel % 2 == 0) ? HIGH : LOW);
  }

  m_joystick.setButton(20, digitalRead(53) != 0);
  m_joystick.setXAxis(m_readMux.ReadChannelValue(5));

  m_joystick.setButton(30, m_readMux.ReadChannelValue(0) != 0);
  m_joystick.setButton(31, m_readMux.ReadChannelValue(1) != 0);

  m_writeMux.WriteChannelValue(0, m_readMux.ReadChannelValue(1) != 0 ? HIGH : LOW);
  m_writeMux.WriteChannelValue(1, m_readMux.ReadChannelValue(4) != 0 ? HIGH : LOW);

  Serial.println();

  m_joystick.sendState();
  delay(10);
}
