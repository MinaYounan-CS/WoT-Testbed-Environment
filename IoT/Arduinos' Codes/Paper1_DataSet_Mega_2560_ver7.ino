// in the name of allah
// Switch Buttons For Controlling Leds
// Conditions are Set on ldr and DHT(LM35)

//#include <IRremote.h>
//#include <IRremoteInt.h>
#include <dht.h>

#define dht_dpin A15 //no ; here. Set equal to channel sensor is on
dht DHT;            //Humidity and Temperature
//-----------------------------------------------------------------------------------------------------------<Board=1_Arduino-UNO>
  String   Board="2_Arduino-Mega2560", Gateway="GW_ID=2";
  String   type="2_Arduino-Mega2560";  int No_Dpin=13, No_Apin=6;
  String   message = "";          // a string to hold incoming data
  boolean  reading = false;      // whether the string is complete
//-------------------------------------------------------------------------------<Variables>
  #define DELAY  150  
  int TestPin=13;               boolean ledstate=false;

  int PWM_LED[5]={5,6,9,10,11}; //Leds on Pins 5,6,9,10,11 can be changed from 0:255
  
  int Led[8]={33,35,37,41,43,45,47,49};       int count_l=8;  
  int LedPrevValue[8]={0,0,0,0,0,0,0,0};      boolean LedPrevState[8]={false,false,false,false,false,false,false,false};  
  int Switch[6]={40,42,44,46,48,38};          int count_s=6;  boolean SwitchPrevState[6]={false,false,false,false,false,false};
  int Fan[1]={39};                            int count_f=1;  boolean FanPrevState[1]={false};
  int FanPrevValue[1]={0}; 
 // int tests=42;
  //int LM35=A15; 
  int LDR=A14; //LM35= DHT  //A0=54 ... A14=68 A15=69
  int LDRPrevValue=0, DHTPrevValue_T=0,DHTPrevValue_H=0;
  
  boolean AllConditions=false;
  boolean FanTempCondition=false;          int     fan_off_temp_down=23;
  boolean LedLdrCondition=false;           int     led_off_ldr_down=23;
  boolean generateDataset=false;           String  Which_To_Feed="";           
  boolean Feed_Temp=false;                 boolean Feed_LDR=false;
  boolean Feed_Changes_Only=false;         int     Feed_Seconds=1000;               
  unsigned long changeTime=millis();       int     dly=100;                      
//-------------------------------------------------------------------------------<SetUp>
void setup()
{
  Serial.begin(9600);                                          // Start the receiver
  pinMode(TestPin,OUTPUT);digitalWrite(TestPin,LOW);           // TestLed
  for(int i=0; i<count_l;i++)                                  // Led PinMode
   { pinMode(Led[i], OUTPUT); }//digitalWrite(Led[i],LOW);}
  for(int i=0; i<count_f;i++)                                  // Fan PinMode
    pinMode(Fan[i], OUTPUT); 
  for(int i=0; i<count_s;i++)                                  // Switch PinMode
    {pinMode(Switch[i], INPUT); digitalWrite(Switch[i],HIGH);} // Pullup Switches
  pinMode(LDR,INPUT);                                          // LDR PinModes   //pinMode(LM35,INPUT);   
  //pinMode(tests,INPUT); digitalWrite(tests,HIGH);
}
//-------------------------------------------------------------------------------<Loop>
void loop()
{
  DHT.read11(dht_dpin); //Read Humidity and Temperature Values
  //Serial.println(DHT.humidity);
// Serial.print(digitalRead(Led[0])); Serial.print(" "); Serial.print(digitalRead(Led[1])); Serial.print(" "); Serial.println(digitalRead(Led[2]));
 //Switch Button Control Conditions to be on/off
 //Serial.println(A0);
    boolean button_pressed ;
    for(int i=0; i<count_s-1;i++)
    {
      button_pressed = handle_button(Switch[i]);
      if(button_pressed==HIGH){ handle_button(i,i);}//  Serial.print(Switch[i]); Serial.print("=");Serial.println(button_pressed);}
    }
    button_pressed = handle_button(Switch[count_s-1]);
    if(button_pressed==HIGH)  AllConditions=true; else AllConditions=false; 
 //-------------------------------------------------------<Reading Msgs>
  // print the string when a newline arrives:
  while(Serial.available()>0)  { reading=true; message=Serial.readStringUntil('\n');}// end while
  //Performing Incomming Message
  if(reading==true)//||(generateDataset==true))
  {
    //messaging Type
    if((message=="#Dataset_on")||(message == Board+":Dataset_on")){generateDataset=true;Serial.println("generateDataset=on");}
    else if((message=="#Quick")||(message == Board+":Quick"))  {set_defaults(); dly=0;  Serial.println("Recieve Quickly");}
    else if((message=="#Normal")||(message == Board+":Normal")){dly=100;Serial.println("Set to Normal");}
    else if((message=="#Dataset_off")||(message == Board+":Dataset_off")){generateDataset=false;Serial.println("generateDataset=off");}
    else if((message=="#Feed_Changes_on")|| (message == Board+":Feed_Changes_on")) {Feed_Changes_Only=true;  Serial.println("Feed_Changes_on");Feed_Seconds=0;}
    else if((message=="#Feed_Changes_off")||(message == Board+":Feed_Changes_off")){Feed_Changes_Only=false; Serial.println("Feed_Changes_off");}
    //time 
    else if((message=="#Time=0")||(message == Board+":Time=0")){Feed_Seconds=0;}// send information according to state changes
    else if(message.indexOf("Time")>0)
            { int x=message.indexOf('=');  x = message.substring(x+1, message.length()).toInt(); Feed_Seconds=x; Feed_Changes_Only=false; Serial.print("time");Serial.println(x);}
    //condition
    else if((message=="#Temp_on")|| (message == Board+":Temp_on"))   {FanTempCondition=true;   Serial.println("Temp_on");}
    else if((message=="#Temp_off")||(message == Board+":Temp_off"))  {FanTempCondition=false;  Serial.println("Temp_off");}
    else if((message=="#LDR_on")||  (message == Board+":LDR_on"))    {LedLdrCondition =true;   Serial.println("LDR_on");}
    else if((message=="#LDR_off")|| (message == Board+":LDR_off"))   {LedLdrCondition =false;  Serial.println("LDR_off");}
    else if((message=="#Feed_Temp_on") ||(message == Board+":Feed_Temp_on")||(message=="#TempSensor")||(message==Board+":TempSensor")) { Feed_Temp=true;  Serial.println("Feed_Temp_on");}
    else if((message=="#Feed_Temp_off")||(message == Board+":Feed_Temp_off")){ Feed_Temp=false; Serial.println("Feed_Temp_off");}
    else if((message=="#Feed_LDR_on")  ||(message == Board+":Feed_LDR_on") ||(message=="#PhotoCell") ||(message==Board+":PhotoCell"))  { Feed_LDR=true;   Serial.println("Feed_LDR_on");}
    else if((message=="#Feed_LDR_off")|| (message == Board+":Feed_LDR_off"))  { Feed_LDR=false;  Serial.println("Feed_LDR_off");}
    //commands
    else if((message == "#Open")|| (message == Board+":Open")) { digitalWrite(TestPin, HIGH); Serial.print(Board+":"); Serial.println("Connected"); } 
    else if((message == "#Close")||(message == Board+":Close")){ digitalWrite(TestPin, LOW);  Serial.print(Board+":"); Serial.println("Disconnected"); set_defaults();}
    else if((message == "#Who")||  (message == Board+":Who")){ Serial.println(Who_I_AM()); digitalWrite(TestPin, HIGH);Serial.print(Board+":"); Serial.println("Connected");}
    //Which To Feed
    else if((message=="#All")||(message == Board+":All"))      { Which_To_Feed="All";    Serial.println("All");}
    else if((message=="#Led")||(message == Board+":Led"))      { Which_To_Feed="Led";    Serial.println("Led");}
    else if((message=="#Fan")||(message == Board+":Fan"))      { Which_To_Feed="Fan";    Serial.println("Fan");}
    else if((message=="#IR")||(message == Board+":IR"))        { Which_To_Feed="";    Serial.println("IR :NO-Code");}
    else if((message=="#BtSwitch")||(message == Board+":BtSwitch")){ Which_To_Feed="Switch"; Serial.println("Switch");}
    else ;
    
    reading=false;  
  }// end if message available or dataset_generation process 
  
 // Do Condition if it is True
  if(LedLdrCondition)  set_led_condition(); 
 // else set_rgb_led(0,0,0); // set RGB led off 
  if(FanTempCondition) set_fan_condition(); 
  
 // DataSet Information
  if(generateDataset==true)
  {
    if(millis()-changeTime>=Feed_Seconds)
    {
      String msg=Board+":"; //Serial.println(changeTime-millis());
      String temp;
      if(Which_To_Feed=="All") 
      {  
        if(Feed_Changes_Only)
        {
          temp=get_switch_states_changes();  if(temp!="NoChange")msg+=temp;  
          temp=get_fan_states_changes();     if(temp!="NoChange")msg+=temp;// { Serial.print(Board+":"); Serial.println(temp);} 
          temp=get_led_states_changes();     if(temp!="NoChange")msg+=temp;// { Serial.print(Board+":"); Serial.println(temp);}
        }
        else
        {
          temp=get_switch_states_all(); msg+=temp;
          temp=get_fan_states_all();    msg+=temp;//Serial.print(Board+":");  Serial.println(get_fan_states_all()); 
          temp=get_led_states_all();    msg+=temp;//Serial.print(Board+":");  Serial.println(get_led_states_all()); 
        }
      }// end all
      else if(Which_To_Feed=="Led")
      {
        if(Feed_Changes_Only){  temp=get_led_states_changes();    if(temp!="NoChange") msg+=temp; }//{ Serial.print(Board+":"); Serial.println(temp); } 
        else {temp=get_led_states_all(); msg+=temp; }//{ Serial.print(Board+":");  Serial.println(get_led_states_all()); }
      }// end led
      else if(Which_To_Feed=="Switch")
      {
        if(Feed_Changes_Only){  temp=get_switch_states_changes(); if(temp!="NoChange") msg+=temp; }//{ Serial.print(Board+":"); Serial.println(temp); }
        else {temp=get_switch_states_all(); msg+=temp; }//{ Serial.print(Board+":");  Serial.println(get_led_states_all()); }
      }// end switch
      else if(Which_To_Feed=="Fan") 
      { 
        if(Feed_Changes_Only){  temp=get_fan_states_changes();    if(temp!="NoChange") msg+=temp; }// {Serial.print(Board+":"); Serial.println(temp); }
        else {temp=get_fan_states_all(); msg+=temp;}//{Serial.print(Board+":");  Serial.println(get_fan_states_all());}
      }// end fan
      
      else{}
    
      if(Feed_LDR)   
        {
          String temp=""; 
          if(Feed_Changes_Only){temp=get_LDR_value_change(); if(temp!="NoChange") msg+=temp;}
          else{ temp=get_LDR_value(); msg+=temp;}
        }
      if(Feed_Temp)  
        {
          String temp="";
          if(Feed_Changes_Only){temp=get_LM_value_change(); if(temp!="NoChange") msg+=temp;}
          else {temp=get_LM_value();  msg+=temp;}
        }
      if(msg!=(Board+":"))      
        Serial.println(msg);
      changeTime=millis();
    }//end if changeTime is equal to interval feed_seconds
  }// end dataset
  
//if 'allconditions' message recieved 
  if(AllConditions)set_conditions();
  else {set_rgb_led(0,0,0); LedPrevValue[0]=0; LedPrevValue[1]=0; LedPrevValue[2]=0; LedPrevState[0]=false; LedPrevState[1]=false;LedPrevState[2]=false;}

  delay(dly);
}//-----------------------------------------------------------------------------------------------------------------------<End Loop>
//.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-<Functions>
//------------------------------------------------------------------------------------------------------------------------<Switch Handling -LM&LDR Conditions>
boolean handle_button(int BUTTON_PIN)
{
  int button_pressed = !digitalRead(BUTTON_PIN); // pin low -> pressed
  return button_pressed;
}
//----------------------------------------------------------------------------------
void handle_button(int SwitchPinIndex,int LedPinIndex)
{
  //int button_pressed = handle_button(SwitchPinIndex);
 // if(button_pressed==HIGH) 
  //{ 
    LedPrevState[LedPinIndex]=!LedPrevState[LedPinIndex]; 
    if(LedPrevState[LedPinIndex]){digitalWrite(Led[LedPinIndex],HIGH);LedPrevState[LedPinIndex]=true;LedPrevValue[LedPinIndex]=255; SwitchPrevState[SwitchPinIndex]=true;} 
    else {digitalWrite(Led[LedPinIndex],LOW);LedPrevState[LedPinIndex]=false; LedPrevValue[LedPinIndex]=0; SwitchPrevState[SwitchPinIndex]=false; }
  //}
}
//-----------------------------------------------------------------------------------------------------------<Who_am_I?>
String Who_I_AM(){ return Gateway; }
//-----------------------------------------------------------------------------------------------------------<Set Defaults>
void set_defaults()
{
 FanTempCondition=false;   LedLdrCondition=false;    AllConditions=false;     Feed_Temp=false;            Feed_LDR=false;
 generateDataset=false;    Feed_Seconds=1000;           Which_To_Feed="";
}
//-----------------------------------------------------------------------------------------------------------<LedPins>
String get_led_list()
{ 
  String pins="";
  for(int i=0; i<count_l-1;i++) { pins+=Led[i]; pins+=","; }
  pins+=Led[count_l-1];
  return pins;
}
//-----------------------------------------------------------------------------------------------------------<FanPins>
String get_fan_list()
{ 
  String pins="";
  for(int i=0; i<count_f-1;i++)  { pins+=Fan[i]; pins+=","; }
  pins+=Fan[count_f-1];
  return pins;
}
//-----------------------------------------------------------------------------------------------------------<FanPins>
String get_switch_list()
{ 
  String pins="";
  for(int i=0; i<count_f-1;i++)  { pins+=Switch[i]; pins+=","; }
  pins+=Switch[count_f-1];
  return pins;
}
//-----------------------------------------------------------------------------------------------------------<RGB LED Control>
void set_rgb_led(int r,int g,int b)
{ digitalWrite(Led[0],b); digitalWrite(Led[1],g);  digitalWrite(Led[2],r); }
//-----------------------------------------------------------------------------------------------------------<Led_Set LDR_Condition>
void set_led_condition()
{
  DHTPrevValue_T= DHT.temperature;
  LDRPrevValue=analogRead(LDR);
  int Light  =1023- LDRPrevValue;
  int LED=Light/4;
  DHTPrevValue_H=DHT.humidity; 
  
  if(DHTPrevValue_H<=70)     { set_rgb_led(0,0,255); LedPrevValue[0]=0; LedPrevState[0]=false;LedPrevValue[1]=0; LedPrevState[1]=false;LedPrevValue[2]=1; LedPrevState[2]=true;}
  else if(DHTPrevValue_H<=75){ set_rgb_led(0,255,0); LedPrevValue[0]=0; LedPrevState[0]=false;LedPrevValue[2]=0; LedPrevState[2]=false;LedPrevValue[1]=1; LedPrevState[1]=true;} 
  else                       { set_rgb_led(255,0,0); LedPrevValue[2]=0; LedPrevState[2]=false;LedPrevValue[1]=0; LedPrevState[1]=false;LedPrevValue[0]=1; LedPrevState[0]=true;}
 
  for(int i=3; i<count_l;i++) 
  {
   if((Led[i]==PWM_LED[0])||(Led[i]==PWM_LED[1])||(Led[i]==PWM_LED[2])||(Led[i]==PWM_LED[3])||(Led[i]==PWM_LED[4]))
       { analogWrite(Led[i],LED); LedPrevValue[i]=LED; }
   else{ digitalWrite(Led[i],LED); if (LED>0) LedPrevValue[i]=1; else LedPrevValue[i]=0; }

   if(LED>0) LedPrevState[i]=true;
   else      LedPrevState[i]=false;
  }
}
//-----------------------------------------------------------------------------------------------------------<Fan_Set LM35_Condition>
void set_fan_condition()
{     
 if(DHT.temperature>=fan_off_temp_down)
  {
     for(int i=0; i<count_f;i++) 
     { digitalWrite(Fan[i],HIGH); FanPrevState[i]=true; FanPrevValue[i]=255;}
  }
  else
  {
    for(int i=0; i<count_f;i++) 
     { digitalWrite(Fan[i],LOW); FanPrevState[i]=false;FanPrevValue[i]=0;}
  }   
}
//-----------------------------------------------------------------------------------------------------------<All Conditions >
void set_conditions()
{ set_led_condition(); set_fan_condition(); }
//-----------------------------------------------------------------------------------------------------------<DHT_Value>
String get_LM_value()
{ 
  String s="@";     int temp=DHT.humidity; 
  s+=temp; s+=".";      temp=DHT.temperature; s+=temp; s+=",";  
  return s; 
}
//-----------------------------------------------------------------------------------------------------------<DHT_Value_Changes>
String get_LM_value_change()
{ 
  String s="";     int h=DHT.humidity; int t=DHT.temperature;
  if ((DHTPrevValue_H!=h) ||(DHTPrevValue_T!=t))
     { DHTPrevValue_H=h; DHTPrevValue_T=t; s+=dht_dpin; s+="="; s+=h; s+="."; s+=t; s+=","; }
  else  s="NoChange";
  return s; 
}
//-----------------------------------------------------------------------------------------------------------<LDR_Value>
String get_LDR_value()
{ String s="@"; LDRPrevValue=analogRead(LDR); s+=LDRPrevValue; s+=","; return s; }
//-----------------------------------------------------------------------------------------------------------<LDR_Value_Changes>
String get_LDR_value_change()
{ 
  String s=""; int x=analogRead(LDR);
  if(LDRPrevValue!=x){ LDRPrevValue=x; s+=LDR; s+="="; s+=LDRPrevValue; s+=",";} else s="NoChange"; return s; 
}
//-----------------------------------------------------------------------------------------------------------<Leds_States>
String get_led_states_all()
{ 
  String pins="@";int x;
  for(int i=0; i<3;i++)
   { x=digitalRead(Led[i]); pins+=x; pins+=",";   }
  for(int i=3; i<count_l;i++)
   { 
     if((Led[i]==PWM_LED[0])||(Led[i]==PWM_LED[1])||(Led[i]==PWM_LED[2])||(Led[i]==PWM_LED[3])||(Led[i]==PWM_LED[4]))
     x=analogRead(Led[i]);
     else
     x=digitalRead(Led[i]);   //pins+=Led[i];  pins+="="; 
     pins+=x; pins+=",";    
   }
  return pins;
}
//-----------------------------------------------------------------------------------------------------------<Leds_States_Changes>
String get_led_states_changes()
{ 
  String pins="";int x;boolean change=false;
  for(int i=0; i<count_l;i++)
   { 
     if((Led[i]==PWM_LED[0])||(Led[i]==PWM_LED[1])||(Led[i]==PWM_LED[2])||(Led[i]==PWM_LED[3])||(Led[i]==PWM_LED[4]))
     x=analogRead(Led[i]);
     else
     x=digitalRead(Led[i]);
     //if(LedPrevState[i]!=x) {pins+=Led[i];  pins+="="; pins+=x; pins+=","; LedPrevState[i]=x; change=true;}  
     if(LedPrevValue[i]!=x)   {pins+=Led[i];  pins+="="; pins+=x; pins+=","; LedPrevValue[i]=x; change=true; if(x>0)LedPrevState[i]=true; else LedPrevState[i]=false;}
   }
   if(change)  return pins;
   else        return "NoChange";
}
//-----------------------------------------------------------------------------------------------------------<Switch_States>
String get_switch_states_all()
{ 
  String pins="@";int x;
  for(int i=0; i<count_s;i++)
   { 
     if(SwitchPrevState[i]) x=1; else x=0;//x=digitalRead(Switch[i]);   //pins+=Switches[i];  pins+="="; 
     pins+=x; pins+=",";    
   }
  return pins;
}
//-----------------------------------------------------------------------------------------------------------<Switch_States_Changes>
String get_switch_states_changes()
{ 
  String pins="";int x;boolean change=false;
  for(int i=0; i<count_s;i++)
   { 
     x=digitalRead(Switch[i]);
     if(SwitchPrevState[i]!=x) {pins+=Switch[i];  pins+="="; pins+=x; pins+=","; SwitchPrevState[i]=x; change=true;}      
   }
   if(change)  return pins;
   else        return "NoChange";
}
//-----------------------------------------------------------------------------------------------------------<Fan_States>
String get_fan_states_all()
{ 
  String pins="@";int x;
  for(int i=0; i<count_f;i++)
   { 
    if((Fan[i]==PWM_LED[0])||(Fan[i]==PWM_LED[1])||(Fan[i]==PWM_LED[2])||(Fan[i]==PWM_LED[3])||(Fan[i]==PWM_LED[4]))
     x=analogRead(Fan[i]);
    else 
     x=digitalRead(Fan[i]); //pins+=Fan[i];  pins+="="; 
     pins+=x; pins+=","; 
   } 
  return pins;
}
//-----------------------------------------------------------------------------------------------------------<Fan_States_Changes>
String get_fan_states_changes()
{ 
  String pins="";
  int x; boolean change=false;
  for(int i=0; i<count_f;i++)
   { 
    if((Fan[i]==PWM_LED[0])||(Fan[i]==PWM_LED[1])||(Fan[i]==PWM_LED[2])||(Fan[i]==PWM_LED[3])||(Fan[i]==PWM_LED[4]))
     x=analogRead(Fan[i]);
     else 
     x=digitalRead(Fan[i]);
     //if(FanPrevState[i]!=x) {pins+=Fan[i];  pins+="="; pins+=x; pins+=","; FanPrevState[i]=x; change=true; }
     if(FanPrevValue[i]!=x) {pins+=Fan[i];  pins+="="; pins+=x; pins+=","; FanPrevState[i]=x; change=true; FanPrevValue[i]=x; if(x>0)FanPrevState[i]=true; else FanPrevState[i]=false;}
 } 
  // pins=pins.substring(0,pins.length-1);
  if(change)  return pins;
  else        return "NoChange";
}
//-----------------------------------------------------------------------------------------------------------<Dpin_SetOn>
void set_dpin_on(int pin)     { digitalWrite(pin,HIGH);}
//-----------------------------------------------------------------------------------------------------------<Dpin_SetOff>
void set_dpin_off(int pin)    { digitalWrite(pin,LOW);}
//-----------------------------------------------------------------------------------------------------------<Apin_State>
String get_apin_state(int pin){ return analogRead(pin)+""; }
//-----------------------------------------------------------------------------------------------------------<Dpin_State>
String get_dpin_state(int pin){ return digitalRead(pin)+""; }//if(digitalRead(pin)>0) "on" else return "off"; }
//-----------------------------------------------------------------------------------------------------------<Leds_PossibleStates>
String get_led_possiblestate(){ return "on,off"; }
//-----------------------------------------------------------------------------------------------------------<End>

