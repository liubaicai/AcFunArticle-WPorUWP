using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Reflection;
using System.Diagnostics;

namespace ACFUN
{
    public partial class App : Application
    {
        /// <summary>
        ///提供对电话应用程序的根框架的轻松访问。
        /// </summary>
        /// <returns>电话应用程序的根框架。</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        public static string AppVersion
        {
            get
            {
                return new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version.ToString();
            }
        }
        public static int AppVersionCount
        {
            get
            {
                var version = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version;
                return version.Major * 1000000 + version.Minor * 10000 + version.Build * 100 + version.Revision;
            }
        }

        /// <summary>
        /// Application 对象的构造函数。
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // 特定于电话的初始化
            InitializePhoneApplication();

            // 调试时显示图形分析信息。
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // 显示当前帧速率计数器。
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // 显示在每个帧中重绘的应用程序区域。
                //Application.Current.Host.Settings.EnableRedrawRegions = true；

                // Enable non-production analysis visualization mode, 
                // 该模式显示递交给 GPU 的包含彩色重叠区的页面区域。
                //Application.Current.Host.Settings.EnableCacheVisualization = true；

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                //  注意: 仅在调试模式下使用此设置。禁用用户空闲检测的应用程序在用户不使用电话时将继续运行
                // 并且消耗电池电量。
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            try
            {
                ThemeManager.ToDarkTheme();
            }
            catch { }
        }

        // 应用程序启动(例如，从“开始”菜单启动)时执行的代码
        // 此代码在重新激活应用程序时不执行
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            UmengSDK.UmengAnalytics.setDebug(false);
            UmengSDK.UmengAnalytics.onLaunching("5222ee2056240b310b086cd6");

#if !DEBUG
            BaicaiStatisticsSDK.BaicaiStatistics.Init("acfun", AppVersion, (isSuccess, err) =>
            {
                if (isSuccess)
                    Debug.WriteLine("BaicaiStatisticsSDK_Init_OK");
                else
                    Debug.WriteLine("BaicaiStatisticsSDK_Init_ERR:" + err.Message);
            }, true);
#endif
        }

        // 激活应用程序(置于前台)时执行的代码
        // 此代码在首次启动应用程序时不执行
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            UmengSDK.UmengAnalytics.onActivated("5222ee2056240b310b086cd6");

            if (StaticData.acitem == null)
            {
                string content = ReadFile("acitem.json");
                if (content != null && content != "")
                {
                    StaticData.acitem = Json.Deserialize<ACItem>(content);
                    Debug.WriteLine("Activated:" + content);
                }
            }
        }

        // 停用应用程序(发送到后台)时执行的代码
        // 此代码在应用程序关闭时不执行
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            if (StaticData.acitem != null)
            {
                string data = Json.Serializer<ACItem>(StaticData.acitem);
                WriteToFile(data, "acitem.json");
                Debug.WriteLine("Deactivated:" + data);
            }
        }

        // 应用程序关闭(例如，用户点击“后退”)时执行的代码
        // 此代码在停用应用程序时不执行
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // 导航失败时执行的代码
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // 导航已失败；强行进入调试器
                System.Diagnostics.Debugger.Break();
            }
        }

        // 出现未处理的异常时执行的代码
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // 出现未处理的异常；强行进入调试器
                System.Diagnostics.Debugger.Break();
            }

            e.Handled = true;

            (RootVisual as Microsoft.Phone.Controls.PhoneApplicationFrame).Source = new Uri("/MainPage.xaml", UriKind.Relative);
        }

        #region 电话应用程序初始化

        // 避免双重初始化
        private bool phoneApplicationInitialized = false;

        // 请勿向此方法中添加任何其他代码
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // 创建框架但先不将它设置为 RootVisual；这允许初始
            // 屏幕保持活动状态，直到准备呈现应用程序时。
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // 处理导航故障
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // 确保我们未再次初始化
            phoneApplicationInitialized = true;
        }

        // 请勿向此方法中添加任何其他代码
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // 设置根视觉效果以允许应用程序呈现
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // 删除此处理程序，因为不再需要它
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion

        private void WriteToFile(string content, string filename)
        {
            try
            {
                // Get the local folder.
                System.IO.IsolatedStorage.IsolatedStorageFile local =
                    System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();

                // Create a new folder named DataFolder.
                if (!local.DirectoryExists("DataFolder"))
                    local.CreateDirectory("DataFolder");

                // Create a new file named DataFile.txt.
                using (var isoFileStream =
                        new System.IO.IsolatedStorage.IsolatedStorageFileStream(
                            "DataFolder\\" + filename,
                            System.IO.FileMode.Create,
                                local))
                {
                    // Write the data from the textbox.
                    using (var isoFileWriter = new System.IO.StreamWriter(isoFileStream))
                    {
                        isoFileWriter.WriteLine(content);
                    }
                }
            }
            catch { }
        }
        private string ReadFile(string filename)
        {
            try
            {
                // Obtain a virtual store for the application.
                System.IO.IsolatedStorage.IsolatedStorageFile local =
                    System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();

                if (local.FileExists("DataFolder\\" + filename))
                {
                    // Specify the file path and options.
                    using (var isoFileStream = new System.IO.IsolatedStorage.IsolatedStorageFileStream
                                ("DataFolder\\" + filename, System.IO.FileMode.Open, local))
                    {
                        // Read the data.
                        using (var isoFileReader = new System.IO.StreamReader(isoFileStream))
                        {
                            return isoFileReader.ReadLine();
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
            catch { return null; }
        }
    }
}