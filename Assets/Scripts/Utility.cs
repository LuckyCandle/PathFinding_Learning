using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    /*  洗牌算法
    1. Fisher-Yates Shuffle算法：将每次随机到的数字存储到一个缓存的list中，在下一次随机时删除这些元素并打乱原先的list
                                                   (缺点：占用了额外的内存空间，空间复杂度高啦)
    2.Knuth-Durstenfeld Shuffle算法：交换元素顺序以及使用队列先进先出的序列进行洗牌（推荐！）
    */
    public static List<Vector2Int> ShuffleCoords(List<Vector2Int> _dataList)
    {
        for (int i = 0; i < _dataList.Count; i++)
        {
            //进阶：这里可以使用噪声作为随机种子
            int randomNum = Random.Range(i, _dataList.Count);
            Vector2Int temp = _dataList[randomNum];
            _dataList[randomNum] = _dataList[i];
            _dataList[i] = temp;
        }
        return _dataList;
    }
}
