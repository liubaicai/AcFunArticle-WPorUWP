BaicaiStatisticsSDK.BaicaiStatistics.Init("test", new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version.ToString(), (isSuccess, err) =>
{
#if DEBUG
    if (isSuccess)
        Debug.WriteLine("BaicaiStatisticsSDK_Init_OK");
    else
        Debug.WriteLine("BaicaiStatisticsSDK_Init_ERR:" + err.Message);
#endif
}, true);