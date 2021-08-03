using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System;

//���ӵĹ��ܣ�ÿN���ӡһЩ��Ϣ
//�¼���ӵ����: timer
//�¼���Elapsed
//�¼�����Ӧ�ߣ�Printer
//�¼����������Զ����¼�����������Ҫ���¼���������һ��
//�¼����Ĺ�ϵ��+=

public class EventTest : MonoBehaviour
{
    Timer timer = new Timer();
    public static int count = 0;

    private void Start()
    {
        //timer.Elapsed����һ���¼������¼�����ֵΪ�գ���������һ��object����һ��ElapsedEventArgs
        timer.Elapsed += Printer.MyAction;
        timer.Interval = 500;
        //�����¼����߼���
        timer.Start();
    }

    public class Printer
    {
        //��Ӧ�¼���EventHandler��������Ĳ������¼�����һ��
        internal static void MyAction(object sender, ElapsedEventArgs e)
        {
            Debug.Log(EventTest.count++);
        }
    }
}
