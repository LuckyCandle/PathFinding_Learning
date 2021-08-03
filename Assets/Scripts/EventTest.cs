using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System;

//例子的功能：每N秒打印一些信息
//事件的拥有者: timer
//事件：Elapsed
//事件的响应者：Printer
//事件处理器：自定义事件处理器，但要与事件参数保持一致
//事件订阅关系：+=

public class EventTest : MonoBehaviour
{
    Timer timer = new Timer();
    public static int count = 0;

    private void Start()
    {
        //timer.Elapsed就是一个事件，该事件返回值为空，两个参数一个object，另一个ElapsedEventArgs
        timer.Elapsed += Printer.MyAction;
        timer.Interval = 500;
        //启动事件（逻辑）
        timer.Start();
    }

    public class Printer
    {
        //响应事件（EventHandler），这里的参数与事件参数一致
        internal static void MyAction(object sender, ElapsedEventArgs e)
        {
            Debug.Log(EventTest.count++);
        }
    }
}
