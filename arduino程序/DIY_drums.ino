/*
* DIY电子架子鼓敲击检测程序
*/
#define N 65 //当一段时间内一直低于音符的最小演奏力度，则认为该音符演奏结束。N：如果连续低于音符的最小演奏力度的次数等于N，则认为该音符演奏结束。
#define PIN_NUMS 6 //需要用到的模拟接口数量，也代表鼓垫数量，模拟接口必须从A0开始依次往后
#define STANDARD_MIN_THRESHOLD 20 //鼓垫的阈值标准

int kickCnt[PIN_NUMS]; //用于统计每个鼓垫分别被敲击了几次
bool PLAY_END[PIN_NUMS]; //用于判断此次演奏的音符是否结束
int lowNumCnt[PIN_NUMS]; //记录此次演奏的音符连续低于音符的最小演奏力度的次数
int MIN_THRESHOLDS[PIN_NUMS];

bool  adjustMode; //调教模式，用于识别哪些引脚是没有连接传感器的，以及获取已经连接上的最小阈值
int runTime = 0;
bool hasConnected[PIN_NUMS]; //代表每个引脚是否有连接的传感器，检测方法是只要在调教模式下找到输入大于30的，则认为是没有连接上（浮空引脚）

void setup() {
  // 31250是MIDI接口的默认传输速率
  Serial.begin(31250);
  adjustMode = true;
  for(int i=0;i<PIN_NUMS;i++){
    //kickCnt[i] = 0;
    PLAY_END[i] = true;
    lowNumCnt[i] = 0;
    MIN_THRESHOLDS[i] = 0;
    hasConnected[i] = true; //一开始默认都是接上的
  }
}

void loop() {
  //调校模式
  if(adjustMode){
    for(int n = 0;n < PIN_NUMS;n++){
      int val = analogRead(n);
      if(val > STANDARD_MIN_THRESHOLD){
        hasConnected[n] = false;
      }
      else{
        if(val > MIN_THRESHOLDS[n]){
          MIN_THRESHOLDS[n] = val;
        }
      }
    }
    runTime++;
    if(runTime > 1000){
      adjustMode = false;
      for(int i = 0;i < PIN_NUMS;i++){
        if(!hasConnected[i]){
          MIN_THRESHOLDS[i] = 1000;
        }
        else{
          MIN_THRESHOLDS[i] += 1;
        }
      }
      return;
    }
    return;
  }
  //正常模式
  for(int i=0;i<PIN_NUMS;i++){
    int val = analogRead(i);
    //敲击一次的判定：出现第一个大于NOTE_MIN_THRESHOLD的值，认为开始敲击；出现第一个小于NOTE_MIN_THRESHOLD连续N次的序列，则认为该音符结束
    if(PLAY_END[i] && val > MIN_THRESHOLDS[i]){
      PLAY_END[i] = false;
      //kickCnt[i]++;
      //Serial.println(i);
      //Serial.println(kickCnt[i]);
      Serial.write(i);
      Serial.flush();
    }
    if(!PLAY_END[i]){
      //每次都判断新值是否小于最低值，如果是，则lowNumCnt+1，当lowNumCnt大于N时，则将PLAY_END置为True；在小于N时，一旦遇到一次大于最低值的，则将lowNumCnt重置
      if(val < MIN_THRESHOLDS[i]){
        lowNumCnt[i]++;
        if(lowNumCnt[i] > N){
          PLAY_END[i] = true;
          lowNumCnt[i] = 0;
        }
      }
      //经实测，这段注释掉效果较好
      // else{
      //   lowNumCnt[i] = 0;
      // }
    }
  }
}
