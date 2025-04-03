const int SLIDER_PIN =5;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);

  pinMode(SLIDER_PIN, INPUT);


}

void loop() {
  // put your main code here, to run repeatedly:
  Serial.print(0);
  
  Serial.print(" ");
  Serial.print(1024);
  Serial.print(" ");

  Serial.println(analogRead(SLIDER_PIN));

  delay(10);
}

