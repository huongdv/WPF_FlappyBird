using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Media;

namespace WpfFlappyBird
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double mScore;　// 点数
        bool mStartGame = false; // ゲーム開始状態
        bool mGameOverFlg = false;　// ゲームオーバー状態
        bool mSpaceKeyPressed = false;　// スペースキー押下状態
        bool mNewPipe1 = true;　// 新しい パイプ１作成状態
        bool mNewPipe2 = true;　// 新しい パイプ２作成状態
        bool mNewPipe3 = true;　// 新しい パイプ３ 作成状態
        int mTimerInterval = 40; // タイマー間隔初期値（miliseconds）
        int mTimerIntervalMin = 20; // タイマー間隔最低値（miliseconds）
        DispatcherTimer mGameTimer = new DispatcherTimer();
        int mBirdGravity = 8;　// 鳥重力
        int mPipeMoveSpeed = 5; // Pipi 移動スピード

        public MainWindow()
        {
            InitializeComponent();

            // タイマー情報
            mGameTimer.Tick += timerCallBack; // タイマー間隔が経過するとコールする
            mGameTimer.Interval = TimeSpan.FromMilliseconds(mTimerInterval);

            // フォーカス設定
            MainCanvas.Focus();

            // 初期化設定
            initGame();
        }

        // 初期化設定
        private void initGame()
        {
            // 点数とフラグ初期化
            mScore = 0;
            mNewPipe1 = true;
            mNewPipe2 = true;
            mNewPipe3 = true;
            mGameOverFlg = false;

            // 点数非表示
            scoreText.Visibility = System.Windows.Visibility.Hidden;

            // "Press Enter to Start"文字表示
            enterToStart.Visibility = System.Windows.Visibility.Visible;

            Canvas.SetTop(flappyBird, 150); // 設定鳥位置 150 pixels

            // 設定デフォルト全部イメージ位置
            foreach (var x in MainCanvas.Children.OfType<Image>())
            {
                // 設定pipe10
                if (x is Image && (string)x.Tag == "pipe10")
                {
                    Canvas.SetLeft(x, 800);
                    Canvas.SetTop(x, -200);
                }
                // 設定pipe11
                if (x is Image && (string)x.Tag == "pipe11")
                {
                    Canvas.SetLeft(x, 800);
                    Canvas.SetTop(x, 310);
                }

                // 設定pipe20
                if (x is Image && (string)x.Tag == "pipe20")
                {
                    Canvas.SetLeft(x, 1150);
                    Canvas.SetTop(x, -200);
                }
                // 設定pipe21
                if (x is Image && (string)x.Tag == "pipe21")
                {
                    Canvas.SetLeft(x, 1150);
                    Canvas.SetTop(x, 310);
                }

                // 設定pipe30
                if (x is Image && (string)x.Tag == "pipe30")
                {
                    Canvas.SetLeft(x, 1500);
                    Canvas.SetTop(x, -200);
                }
                // 設定pipe31
                if (x is Image && (string)x.Tag == "pipe31")
                {
                    Canvas.SetLeft(x, 1500);
                    Canvas.SetTop(x, 310);
                }
            }
        }

        // キーダウン
        private void onPressKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && mStartGame == true)
            {
                // -15 度の回転
                flappyBird.RenderTransform = new RotateTransform(-15, flappyBird.Width / 2, flappyBird.Height / 2);
                // 上に移動
                mBirdGravity = -8;
                mSpaceKeyPressed = true;
            }
        }

        // キーアップ
        private void onPressKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && mStartGame == false)
            {
                startGame();
            }
            if (e.Key == Key.Space && mStartGame == true)
            {
                // 下に移動
                mBirdGravity = 8;

                mSpaceKeyPressed = false;
            }
            if (e.Key == Key.R && mGameOverFlg == true)
            {
                // 初期化設定
                initGame();

                // ゲーム開始
                startGame();
            }
        }

        // ゲーム開始
        private void startGame()
        {
            mStartGame = true;

            // 点数表示
            scoreText.Visibility = System.Windows.Visibility.Visible;

            // ゲーム開始のため、非表示
            GameReady.Visibility = System.Windows.Visibility.Hidden;
            enterToStart.Visibility = System.Windows.Visibility.Hidden;

            // タイマー初期化
            mTimerInterval = 40;
            mGameTimer.Interval = TimeSpan.FromMilliseconds(mTimerInterval);

            mGameTimer.Start(); // タイマー起動
        }

        // タイマーコールバック
        private void timerCallBack(object sender, EventArgs e)
        {
            // 新しい鳥矩形取得
            Rect mNewBirdRect = new Rect(Canvas.GetLeft(flappyBird), Canvas.GetTop(flappyBird), flappyBird.Width, flappyBird.Height);

            // 点数更新
            scoreText.Content = mScore;

            if (mSpaceKeyPressed == false) 
            {
                // // 10 度の回転
                flappyBird.RenderTransform = new RotateTransform(10, flappyBird.Width / 2, flappyBird.Height / 2);
            }

            // 鳥のtop位置設定
            Canvas.SetTop(flappyBird, Canvas.GetTop(flappyBird) + mBirdGravity);

            // 鳥の位置は画面の範囲外の場合
            if (Canvas.GetTop(flappyBird) + flappyBird.Height > 470 || Canvas.GetTop(flappyBird) < 0)
            {
                // ゲームオーバー
                gameOver();
                return;
            }

            // 全部イメージチェック
            foreach (var x in MainCanvas.Children.OfType<Image>())
            {
                if ((string)x.Tag == "base")
                {
                    // 左の画面に移動する（pipeと同じスピード）
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - mPipeMoveSpeed);
                }

                if ( (string)x.Tag == "pipe10" || (string)x.Tag == "pipe11" ||
                     (string)x.Tag == "pipe20" || (string)x.Tag == "pipe21" ||
                     (string)x.Tag == "pipe30" || (string)x.Tag == "pipe31" )
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - mPipeMoveSpeed);

                    // pipe 矩形
                    Rect pipeRect = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    // flappy 矩形 と pipe 矩形　は 衝突
                    if (mNewBirdRect.IntersectsWith(pipeRect))
                    {
                        // ゲームオーバー
                        gameOver();
                        return;
                    }
                }

                // 点数追加
                addNewScore(x);

                //  再設定イメージ位置
                reSetImagePosition(x);
            }
        }

        // 点数追加
        private void addNewScore( Image x )
        {
            bool addScore = false;

            if ((string)x.Tag == "pipe10" && Canvas.GetLeft(x) + pipe10.Width < Canvas.GetLeft(flappyBird) && mNewPipe1 == true)
            {
                mNewPipe1 = false;
                addScore = true;
            }

            if ((string)x.Tag == "pipe20" && Canvas.GetLeft(x) + pipe20.Width < Canvas.GetLeft(flappyBird) && mNewPipe2 == true)
            {
                mNewPipe2 = false;
                addScore = true;
            }

            if ((string)x.Tag == "pipe30" && Canvas.GetLeft(x) + pipe30.Width < Canvas.GetLeft(flappyBird) && mNewPipe3 == true)
            {
                mNewPipe3 = false;
                addScore = true;
            }

            // 点数追加の場合
            if (addScore == true)
            {
                // １点追加と音声出力
                mScore = mScore + 1;
                playSound(1);

                // タイマー間隔調整
                if (mScore % 10 == 0 && mTimerInterval > mTimerIntervalMin)
                {
                    mTimerInterval = mTimerInterval - 1;
                    mGameTimer.Interval = TimeSpan.FromMilliseconds(mTimerInterval);
                }
            }
        }

        //  設定イメージ位置
        private void reSetImagePosition(Image x)
        {
            // pipe1の位置再設定
            if ((string)x.Tag == "pipe10" && Canvas.GetLeft(x) < -100)
            {
                mNewPipe1 = true;
                //　Set Top
                Random random = new Random();
                int setTop = random.Next(-360, -80);
                Canvas.SetTop(x, setTop);
                Canvas.SetTop(pipe11, setTop + 510);

                // Set Left
                Canvas.SetLeft(x, Canvas.GetLeft(pipe30) + 350);
                Canvas.SetLeft(pipe11, Canvas.GetLeft(x) + 5);
            }

            // pipe2の位置再設定
            if ((string)x.Tag == "pipe20" && Canvas.GetLeft(x) < -100)
            {
                mNewPipe2 = true;
                // Set Top
                Random random = new Random();
                int setTop = random.Next(-360, -80);
                Canvas.SetTop(x, setTop);
                Canvas.SetTop(pipe21, setTop + 510);

                // Set Left
                Canvas.SetLeft(x, Canvas.GetLeft(pipe10) + 350);
                Canvas.SetLeft(pipe21, Canvas.GetLeft(x) + 5);
            }

            // pipe3の位置再設定
            if ((string)x.Tag == "pipe30" && Canvas.GetLeft(x) < -100)
            {
                mNewPipe3 = true;
                Random random = new Random();
                int setTop = random.Next(-360, -80);

                Canvas.SetTop(x, setTop);
                Canvas.SetTop(pipe31, setTop + 510);

                Canvas.SetLeft(x, Canvas.GetLeft(pipe20) + 350);
                Canvas.SetLeft(pipe31, Canvas.GetLeft(x) + 5);
            }

            // イメージは"base"の場合、表示位置の左を調整する
            if ((string)x.Tag == "base")
            {
                if (Canvas.GetLeft(x) <= -100)
                {
                    Canvas.SetLeft(x, 0);
                }
            }
        }

        // 音声出力
        private void playSound(int kind)
        {
            SoundPlayer sound;
            switch (kind)
            {
                case 1: // ポイントを集める
                    sound = new SoundPlayer(WpfApplication3.Properties.Resources.collect_point);
                    sound.Play();
                    break;
                case 2: // ゲームオーバー
                    sound = new SoundPlayer(WpfApplication3.Properties.Resources.game_over);
                    sound.Play();
                    break;
                default:
                    break;
            }
        }

        // ゲームオーバー
        private void gameOver()
        {
            mGameTimer.Stop();
            mGameOverFlg = true;
            enterToStart.Content = "Press R to Try Again";
            enterToStart.Visibility = System.Windows.Visibility.Visible;
            playSound(2);
        }
    }
}
