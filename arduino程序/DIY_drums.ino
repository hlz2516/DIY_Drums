/*
* DIY电子架子鼓敲击检测程序
*/
#define N 65 //当一段时间内一直低于音符的最小演奏力度，则认为该音符演奏结束。N：如果连续低于音符的最小演奏力度的次数等于N，则认为该音符演奏结束。
#define PIN_NUMS 5 //需要用到的模拟接口数量，也代表鼓垫数量，模拟接口必须从A0开始依次往后

int kickCnt[PIN_NUMS]; //用于统计每个鼓垫分别被敲击了几次
bool PLAY_END[PIN_NUMS]; //用于判断此次演奏的音符是否结束
int lowNumCnt[PIN_NUMS]; //记录此次演奏的音符连续低于音符的最小演奏力度的次数
int MIN_THRESHOLDS[PIN_NUMS];

void setup() {
  // 31250是MIDI接口的默认传输速率
  Serial.begin(31250);
  for(int i=0;i<PIN_NUMS;i++){
    //kickCnt[i] = 0;
    PLAY_END[i] = true;
    lowNumCnt[i] = 0;
    MIN_THRESHOLDS[i] = 1000; //屏蔽浮空引脚
  }
  //根据自己选择的接口放开引脚输入检测
  MIN_THRESHOLDS[0] = 5;
  MIN_THRESHOLDS[2] = 15;
  MIN_THRESHOLDS[4] = 15;
}

void loop() {
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
