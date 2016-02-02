// in the name of allah
// IR receiver - Type:'strong' of resever remot control
// button can put conditions off and one but remote control can' turn off condition if it is opened by button switch
// remote control can turn conditions on and off
// Messages about devices will be sent ordered by device's titles 
#include <IRremote.h>
#include <IRremoteInt.h>

//-----------------------------------------------------------------------------------------------------------<Board=1_Arduino-UNO>
  String   Board="1_Arduino-UNO", Gateway="GW_ID=1";
  String   type="1_Arduino-UNO";  int No_Dpin=13, No_Apin=6;
  String   message = "";          // a string to hold incoming data
  boolean  reading = false;      // whether the string is complete
//-------------------------------------------------------------------------------<Variables>
  #define DELAY  150  
  int TestPin=13;               boolean ledstate=false;
  int receiver = 3;             int IR_Value=0;     int IRPrevValue=0;                      
  IRrecv irrecv(receiver);      decode_results results;   
  
  int PWM_LED[5]={5,6,9,10,11}; //Leds on Pins 5,6,9,10,11 can be changed from 0:255
  int Led[5]={8,9,10,11,12};    int count_l=5;      boolean LedPrevState[5]={false,false,false,false,false};  
  int LedPrevValue[5]={0,0,0,0,0};
  int Switch[1]={7};            int count_s=1;      boolean SwitchPrevState[1]={false};
  int Fan[0]={};                int count_f=0;      boolean FanPrevState[0]={};
  int FanPrevValue[0]={}; 
 
  int LM35=A0, LDR=A1;          int LM35PrevValue=0,LDRPrevValue=0; //A0=14 A1=15 and so on
  
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
  Serial.begin(9600);
  irrecv.enableIRIn();                                             // Start the receiver
  pinMode(TestPin,OUTPUT);digitalWrite(TestPin,LOW);               // TestLed
  for(int i=0; i<count_l;i++)                                      // Led PinMode
   { pinMode(Led[i], OUTPUT); }//digitalWrite(Led[i],LOW);}
  for(int i=0; i<count_f;i++)                                      // Fan PinMode
    pinMode(Fan[i], OUTPUT); 
  for(int i=0; i<count_s;i++)                                      // Switch PinMode
    {pinMode(Switch[i], INPUT); digitalWrite(Switch[i],HIGH);} // Pullup Switches
  pinMode(LM35,INPUT);   pinMode(LDR,INPUT);                       // LM and LDR PinModes   
}
//-------------------------------------------------------------------------------<Loop>
void loop()
{
 //Switch Button Control Conditions to be on/off
    boolean button_pressed = handle_button(Switch[0]);
    //Serial.println(digitalRead(Switch[0]));
 //Serial.println(LM35);   
    if(button_pressed==HIGH) 
    {  
    
      SwitchPrevState[0]=!  SwitchPrevState[0]; 
      if (SwitchPrevState[0])AllConditions=true; else AllConditions=false; 
      if(!SwitchPrevState[0])
      { irrecv.enableIRIn();}
     // irrecv.enableIRIn(); // refresh IR reciever to read next msg 
    }
 //IR Handling
  if (irrecv.decode(&results))
  {   
    translateIR(); //Serial.print("IR="); Serial.println(IR_Value); Serial.println(digitalRead(Switch[0]));   
    if(IR_Value==10)// testled
      handle_IR(ledstate,TestPin);
    else if(IR_Value==11)// turn on and off conditions
     { 
      SwitchPrevState[0]=!  SwitchPrevState[0]; 
      if (SwitchPrevState[0])AllConditions=true; else AllConditions=false; 
      if(!SwitchPrevState[0]) { irrecv.enableIRIn();}
     }   
    else if(IR_Value==1)
      handle_IR(0);
    else if(IR_Value==2)
      handle_IR(1);
    else if(IR_Value==3)
      handle_IR(2);
    else if(IR_Value==4)
      handle_IR(3);
    else if(IR_Value==5)
      handle_IR(4);
 
    else;
    irrecv.resume(); // receive the next value
  } // end if 
  //-------------------------------------------------------<Reading Msgs>
  // print the string when a newline arrives:
  while(Serial.available()>0)  { reading=true; message=Serial.readStringUntil('\n');}// end while
  //Performing Incomming Message
  if(reading==true)//||(generateDataset==true))
  {
    //messaging Type
    //Serial.println(message);
    
    if((message=="#Dataset_on") ||(message == Board+":Dataset_on")){generateDataset=true;  Serial.println("generateDataset=on");}
    else if((message=="#Quick") ||(message == Board+":Quick"))     {set_defaults(); dly=0; Serial.println("Recieve Quickly");}
    else if((message=="#Normal")||(message == Board+":Normal"))    {dly=100;               Serial.println("Set to Normal");}
    else if((message=="#Dataset_off")||(message == Board+":Dataset_off")){generateDataset=false;Serial.println("generateDataset=off");}
    else if((message=="#Feed_Changes_on") ||(message == Board+":Feed_Changes_on")) {Feed_Changes_Only=true;  Serial.println("Feed_Changes_on"); Feed_Seconds=0;}
    else if((message=="#Feed_Changes_off")||(message == Board+":Feed_Changes_off")){Feed_Changes_Only=false; Serial.println("Feed_Changes_off");}
    //time 
    else if((message=="#Time=0")||(message == Board+":Time=0")){Feed_Seconds=0;}// send information according to state changes
    else if(message.indexOf("Time")>0)
          { int x=message.indexOf('=');  x = message.substring(x+1, message.length()).toInt(); Feed_Seconds=x; 
            Feed_Changes_Only=false; Serial.print("time");Serial.println(x);}
    //condition
    else if((message=="#Temp_on")|| (message == Board+":Temp_on"))   {FanTempCondition=true;   Serial.println("Temp_on");}
    else if((message=="#Temp_off")||(message == Board+":Temp_off"))  {FanTempCondition=false;  Serial.println("Temp_off");}
    else if((message=="#LDR_on")||  (message == Board+":LDR_on"))    {LedLdrCondition =true;   Serial.println("LDR_on");}
    else if((message=="#LDR_off")|| (message == Board+":LDR_off"))   {LedLdrCondition =false;  Serial.println("LDR_off");}
    else if((message=="#Feed_Temp_on") ||(message == Board+":Feed_Temp_on")||(message=="#TempSensor")||(message==Board+":TempSensor")){ Feed_Temp=true; Serial.println("Feed_Temp_on");}
    else if((message=="#Feed_Temp_off")||(message == Board+":Feed_Temp_off")){Feed_Temp=false; Serial.println("Feed_Temp_off");}
    else if((message=="#Feed_LDR_on")  ||(message == Board+":Feed_LDR_on") ||(message=="#PhotoCell") ||(message==Board+":PhotoCell")) { Feed_LDR=true;  Serial.println("Feed_LDR_on");}
    else if((message=="#Feed_LDR_off") ||(message == Board+":Feed_LDR_off" )){Feed_LDR=false;  Serial.println("Feed_LDR_off"); }
    //commands
    else if((message == "#Open")|| (message == Board+":Open")) { digitalWrite(TestPin, HIGH); Serial.print(Board+":"); Serial.println("Connected"); } 
    else if((message == "#Close")||(message == Board+":Close")){ digitalWrite(TestPin, LOW);  Serial.print(Board+":"); Serial.println("Disconnected"); set_defaults();}
    else if((message == "#Who")||  (message == Board+":Who")){ Serial.println(Who_I_AM()); digitalWrite(TestPin, HIGH);Serial.print(Board+":"); Serial.println("Connected");}
    //Which To Feed
    else if((message=="#All")||(message == Board+":All"))          { Which_To_Feed="All";    Serial.println("All");}//  Feed_Temp=true; Feed_LDR=true; 
    else if((message=="#Led")||(message == Board+":Led"))          { Which_To_Feed="Led";    Serial.println("Led");}
    else if((message=="#Fan")||(message == Board+":Fan"))          { Which_To_Feed="Fan";    Serial.println("Fan");}
    else if((message=="#BtSwitch")||(message == Board+":BtSwitch")){ Which_To_Feed="Switch"; Serial.println("Switch");}
    else if((message=="#IR")||(message == Board+":IR"))            { Which_To_Feed="IR";     Serial.println("IR");}
    else ;// Serial.println("It has no code to handel");
    
    reading=false;  
  }// end if message available or dataset_generation process 
  
 // Do Condition if it is True
  if(LedLdrCondition)  set_led_condition(); 
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
          temp=get_fan_states_changes();     if(temp!="NoChange")msg+=temp;
          temp=get_ir_states_changes();      if(temp!="NoChange")msg+=temp;
          temp=get_led_states_changes();     if(temp!="NoChange")msg+=temp;// { Serial.print(Board+":"); Serial.println(temp);}
        }
        else
        {
          temp=get_switch_states_all(); msg+=temp; 
          temp=get_fan_states_all();    msg+=temp;//Serial.print(Board+":");  Serial.println(get_fan_states_all()); 
          temp=get_ir_states_all();     msg+=temp;
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
      else if(Which_To_Feed=="IR") 
      { 
        if(Feed_Changes_Only){  temp=get_ir_states_changes();    if(temp!="NoChange") msg+=temp; }// {Serial.print(Board+":"); Serial.println(temp); }
        else {temp=get_ir_states_all(); msg+=temp;}//{Serial.print(Board+":");  Serial.println(get_fan_states_all());}
      }// end IR
      else{}
    
      if(Feed_LDR)   
        {
          String temp=""; 
          if(Feed_Changes_Only){temp=get_LDR_value_change(); if(temp!="NoChange") msg+=temp;}
          else{temp=get_LDR_value(); msg+=temp;}
        }
      if(Feed_Temp)  
        {
          String temp="";
          if(Feed_Changes_Only){temp=get_LM_value_change(); if(temp!="NoChange") msg+=temp;}
          else {temp=get_LM_value();  msg+=temp;}
        //  Serial.println("Hello");
        //  Serial.println(get_LM_value());
        }
      if(msg!=(Board+":"))  
        Serial.println(msg);
      changeTime=millis();
    }//end if changeTime is equal to interval feed_seconds
  }// end dataset
  
//if 'allconditions' message recieved 
  if(AllConditions)set_conditions();


  delay(dly);
}//-----------------------------------------------------------------------------------------------------------------------<End Loop>
//.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-<Functions>

//------------------------------------------------------------------------------------------------------------------------<Switch Handling -LM&LDR Conditions>
boolean handle_button(int BUTTON_PIN)
{  int button_pressed = !digitalRead(BUTTON_PIN);  return button_pressed; } // pin low -> pressed
//----------------------------------------------------------------------------------
void handle_button(int &Inputpin,boolean & state,int &Outpin,int & lstate)
{
  int button_pressed = handle_button(Inputpin);
  if(button_pressed==HIGH) 
  { lstate=!lstate; if(state)digitalWrite(Outpin,HIGH); else digitalWrite(Outpin,LOW); }
}
//------------------------------------------------------------------------------------------------------------------------<IR Handling -Leds>
void handle_IR(boolean & state,int Outpin)
{ state=!state; if(state)digitalWrite(Outpin,HIGH); else digitalWrite(Outpin,LOW); }
//----------------------------------------------------------------------------------
void handle_IR(int i)
{ 
  LedPrevState[i]=!LedPrevState[i]; 
  if(LedPrevState[i]){digitalWrite(Led[i],HIGH); LedPrevValue[i]=255;}
  else               {digitalWrite(Led[i],LOW);  LedPrevValue[i]=0;}
}
//----------------------------------------------------------------------------------<IR Translator>
void translateIR()        // takes action based on IR code received
{                        // describing KEYES Remote IR codes 
  switch(results.value)
  {
    case 2155806975: IR_Value=10;  break;//Serial.println("PWR");  break;  
    case 2155855935: IR_Value=11;  break;//Serial.println("SND");  break;
    case 2155825335: IR_Value=20;  break;//Serial.println("OK");   break;  
    case 2155849815: IR_Value=21;  break;//Serial.println("UP");   break;
    case 2155833495: IR_Value=22;  break;//Serial.println("DN");   break;
    case 2155866135: IR_Value=23;  break;//Serial.println("LT");   break;
    case 2155817175: IR_Value=24;  break;//Serial.println("RT");   break;
    case 2155839615: IR_Value=1;   break;//Serial.println("1");    break;
    case 2155823295: IR_Value=2;   break;//Serial.println("2");    break;
    case 2155831455: IR_Value=3;   break;//Serial.println("3");    break;
    case 2155815135: IR_Value=4;   break;//Serial.println("4");    break;
    case 2155847775: IR_Value=5;   break;//Serial.println("5");    break;
    case 2155864095: IR_Value=6;   break;//Serial.println("6");    break;
    case 2155811055: IR_Value=7;   break;//Serial.println("7");    break;
    case 2155827375: IR_Value=8;   break;//Serial.println("8");    break;
    case 2155860015: IR_Value=9;   break;//Serial.println("9");    break;
    case 2155835535: IR_Value=0;   break;//Serial.println("0");    break;
    case 2155819215: IR_Value=30;  break;//Serial.println("TVRAD");break; // ON|OFF Light Condition
    case 2155868175: IR_Value=31;  break;//Serial.println("TVSAT");break; // ON|OFF Temp. Condition
    default: break;//Serial.println("OTHER");
  }// End Case
  delay(500); // Do not get immediate repeat
} //END translateIR

//-----------------------------------------------------------------------------------------------------------<Who_am_I?>
String Who_I_AM(){ return Gateway; }
//-----------------------------------------------------------------------------------------------------------<Set Defaults>
void set_defaults()
{
 FanTempCondition=false;   LedLdrCondition=false;    AllConditions=false;    Feed_Temp=false;            Feed_LDR=false;
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
//-----------------------------------------------------------------------------------------------------------<Led_Set LDR_Condition>
void set_led_condition()
{
  for(int i=0; i<count_l;i++)
    if(analogRead(LDR)>=led_off_ldr_down)  { digitalWrite(Led[i],LOW); }
}
//-----------------------------------------------------------------------------------------------------------<Fan_Set LM35_Condition>
void set_fan_condition()
{ 
  if(analogRead(LM35)>=fan_off_temp_down) 
    for(int i=0; i<count_f;i++) digitalWrite(Fan[i],HIGH);  
  else 
    for(int i=0; i<count_f;i++) digitalWrite(Fan[i],LOW); 
}
//-----------------------------------------------------------------------------------------------------------<All Conditions >
void set_conditions()
{
  int Temp= analogRead(LM35); Temp= (Temp*0.0049)*100;
  int Light  =1023- analogRead(LDR);
  int LED=Light/4;
 // Serial.print("Temp=");Serial.print(Temp);Serial.print("Darkness=");Serial.print(Light);Serial.print(" LED_ON_Level=");Serial.println(LED);
  for(int i=0; i<count_l;i++) 
  {
   if((Led[i]==PWM_LED[0])||(Led[i]==PWM_LED[1])||(Led[i]==PWM_LED[2])||(Led[i]==PWM_LED[3])||(Led[i]==PWM_LED[4]))
       { analogWrite(Led[i],LED);}//  Serial.print(" CurLed="); Serial.println(analogRead(Led[i]));}
   else{ digitalWrite(Led[i],LED);}// Serial.print(" CurLed="); Serial.println(digitalRead(Led[i]));}
   LedPrevValue[i]=LED;
   if(LED>0) LedPrevState[i]=true;
   else      LedPrevState[i]=false;
  }
  if(Temp>fan_off_temp_down)
  {
     for(int i=0; i<count_f;i++) 
     { digitalWrite(Fan[i],HIGH); FanPrevState[i]=true;}
  }
  else
  {
    for(int i=0; i<count_f;i++) 
     { digitalWrite(Fan[i],LOW); FanPrevState[i]=false;}
  }
}
//-----------------------------------------------------------------------------------------------------------<LM35_Value_all>
String get_LM_value()
{ 
  String s="@";//"Temp:";//s+=LM35; s+="="; 
  s+=analogRead(LM35); s+=",";
  //Serial.println(s);
  return s;
}
//-----------------------------------------------------------------------------------------------------------<LM35_Value_changes>
String get_LM_value_change()
{ 
  String s="";//"Temp:";//s+=LM35; s+="="; 
  int x=analogRead(LM35); if(x!=LM35PrevValue) {s+=LM35; s+="="; s+=x; s+=",";} else s="NoChange";
  return s;
}
//-----------------------------------------------------------------------------------------------------------<LDR_Value_all>
String get_LDR_value()
{ 
  String s="@";//"LDR:"; //s+=LDR; s+="=";
  s+=analogRead(LDR); s+=",";  
  return s;
}
//-----------------------------------------------------------------------------------------------------------<LDR_Value_changes>
String get_LDR_value_change()
{ 
  String s="";//"Temp:";//s+=LM35; s+="="; 
  int x=analogRead(LDR); if(x!=LDRPrevValue) {s+=LDR; s+="="; s+=x; s+=",";} else s="NoChange";
  return s;
}
//-----------------------------------------------------------------------------------------------------------<IR_States_all>
String get_ir_states_all()
{ String pins="@"; pins+=IR_Value; pins+=","; return pins;}
//-----------------------------------------------------------------------------------------------------------<IR_States changes>
String get_ir_states_changes()
{
  String pins=""; 
  if (IR_Value!=IRPrevValue) {pins+=receiver; pins+="="; pins+=IR_Value; pins+=","; return pins;} 
  else        return "NoChange";
}
//-----------------------------------------------------------------------------------------------------------<Leds_States_all>
String get_led_states_all()
{ 
  String pins="@";int x;
  for(int i=0; i<count_l;i++)
   { 
     if ((AllConditions==true)&&((Led[i]==PWM_LED[0])||(Led[i]==PWM_LED[1])||(Led[i]==PWM_LED[2])||(Led[i]==PWM_LED[3])||(Led[i]==PWM_LED[4])))
       x=analogRead(Led[i]);
     else
       x=digitalRead(Led[i]);   //pins+=Led[i];  pins+="="; 
     pins+=x; pins+=",";    
   }
  return pins;
}
//-----------------------------------------------------------------------------------------------------------<Leds_States_changes>
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
     if(LedPrevValue[i]!=x)   
       {
         pins+=Led[i];  pins+="="; pins+=x; pins+=","; LedPrevValue[i]=x; change=true; 
         if(x>0)LedPrevState[i]=true; else LedPrevState[i]=false;
       }
   }
   if(change)  return pins;
   else        return "NoChange";
}
//-----------------------------------------------------------------------------------------------------------<Switch_States_all>
String get_switch_states_all()
{ 
  String pins="@";int x;
  for(int i=0; i<count_s;i++)
   { 
     if(SwitchPrevState[i]) x=1; else x=0;//digitalRead(Switch[i]);   //pins+=Switches[i];  pins+="="; 
     pins+=x; pins+=",";    
   }
  return pins;
}
//-----------------------------------------------------------------------------------------------------------<Switch_States_changes>
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
//-----------------------------------------------------------------------------------------------------------<Fan_States_all>
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
//-----------------------------------------------------------------------------------------------------------<Fan_States_changes>
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
    if(FanPrevValue[i]!=x) 
     {
       pins+=Fan[i];  pins+="="; pins+=x; pins+=","; FanPrevState[i]=x; change=true; FanPrevValue[i]=x; 
       if(x>0)FanPrevState[i]=true; else FanPrevState[i]=false;
     }
 } 
  // pins=pins.substring(0,pins.length-1);
  if(change)  return pins;
  else        return "NoChange";
}
//-----------------------------------------------------------------------------------------------------------<Dpin_SetOn>
void set_dpin_on(int pin)     { digitalWrite(pin,HIGH);}
//-----------------------------------------------------------------------------------------------------------<Dpin_SetOff>
void set_dpin_off(int pin)    { digitalWrite(pin,LOW); }
//-----------------------------------------------------------------------------------------------------------<Apin_State>
String get_apin_state(int pin){ return analogRead(pin)+"";  }
//-----------------------------------------------------------------------------------------------------------<Dpin_State>
String get_dpin_state(int pin){ return digitalRead(pin)+""; }//if(digitalRead(pin)>0) "on" else return "off"; }
//-----------------------------------------------------------------------------------------------------------<Leds_PossibleStates>
String get_led_possiblestate(){ return "on,off"; }
//-----------------------------------------------------------------------------------------------------------<End>

