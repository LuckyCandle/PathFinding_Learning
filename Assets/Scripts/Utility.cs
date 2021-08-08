using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    /*  ϴ���㷨
    1. Fisher-Yates Shuffle�㷨����ÿ������������ִ洢��һ�������list�У�����һ�����ʱɾ����ЩԪ�ز�����ԭ�ȵ�list
                                                   (ȱ�㣺ռ���˶�����ڴ�ռ䣬�ռ临�Ӷȸ���)
    2.Knuth-Durstenfeld Shuffle�㷨������Ԫ��˳���Լ�ʹ�ö����Ƚ��ȳ������н���ϴ�ƣ��Ƽ�����
    */
    public static List<Vector2Int> ShuffleCoords(List<Vector2Int> _dataList)
    {
        for (int i = 0; i < _dataList.Count; i++)
        {
            //���ף��������ʹ��������Ϊ�������
            int randomNum = Random.Range(i, _dataList.Count);
            Vector2Int temp = _dataList[randomNum];
            _dataList[randomNum] = _dataList[i];
            _dataList[i] = temp;
        }
        return _dataList;
    }
}
