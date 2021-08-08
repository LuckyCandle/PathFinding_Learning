# 寻路算法可视化、随机地图与Navmesh实践

本项目主要参照了[*Red Blob Games*](https://www.redblobgames.com/)相关寻路算法的文章，以及借鉴了[BeaverJoe的广度优先搜索](https://www.bilibili.com/video/BV1X54y1D7Z4)



## 寻路算法可视化

我们的主要目标是要完成BFS,Dijkstra,A*算法，以及他们的可视化。

### 地图生成

地图的生成我主要是根据行列生成多个Cube，每一个Cube的坐标值标准化得到一个Vector2Int值，并用字典类记录WayPointDic\<Vector2Int,Waypoint\>,其实主要目的就是完成了邻接矩阵，以及矩阵中每一个节点与其GameObject的对应关系。我这里还要使用cost来指出通过该节点的消耗,并根据颜色深浅来区分，同时在初始化的过程中会随机对一些Cube赋予isBrick布尔值，使其变为一堵墙，寻路算法不可通过。

![image-20210803212128268](image/image-20210803212128268.png)

灰色颜色区域的都是可通过的路径，黄色为墙，灰色区域颜色越深代表cost越大。

### 数据结构

这里分享一下Waypoint图节点的数据结构

```C#
public class Waypoint{
	public bool isExplored = false;    //是否被访问
    public bool isBrick = false;	   //是否是墙
    public Waypoint from;              //访问他的前一个节点
    public int cost;				   //通过该点的消耗
    public int cost_so_far;            //寻路算法中已经累计的消耗
    public int distance;			   //A*算法中 累计消耗+曼哈顿距离
    public Vector2Int vector2Int;	   //保存该GameObject的Vector2Int值
}
```

### 算法逻辑

这里并不过多的赘述BFS,Dijkstra,A*的实现方法，具体请见Document中的**寻路系统基础.md**。

这里逻辑主要是我们通过鼠标的射线检测获取起始点和终点，在得到两点后我们开始执行寻路算法。

![image-20210803212742605](image/image-20210803212742605.png)

我们的遍历方向是上右下左，这里是通过Vector2Int自带的函数实现

```c#
private Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
//对应的就是(0,1);(1,0);(0,-1);(-1,0)
```

得到路径后通过不停的寻找节点的from来取得path列表如下所示：

```C#
 Waypoint prePoint = endPoint.GetComponent<Waypoint>().from;
 while (prePoint != startPoint.GetComponent<Waypoint>()) {
     prePoint.GetComponent<MeshRenderer>().material.color = Color.yellow;
     prePoint = prePoint.from;
 }
```

则得到路径后会通过黄色高亮出最短路径。

![image-20210803213220586](image/image-20210803213220586.png)

当使用Dijkstra，A*带有cost值得算法时，路径会根据cost值有深浅的变化，更明显地看出最短路径。

![image-20210803213408712](image/image-20210803213408712.png)

### 可视化

要做到寻路算法可视化的方式是要使用unity中的协程函数，而在我们实际的寻路使用中，也可以启动多个协程用于执行不同单位的寻路策略。Unity是单线程的，协程就是在单线程下的一种更轻量的线程，我们可以通过Coroutine创建对象的方式创建协程，也可以通过StartCoroutine(函数)的方式直接调用协程。协程往往会在Start()生命周期中被创建，并开始执行协程，在协程执行的函数体中，我们用yield来控制协程的执行频率，yield return的生命周期在Update()之后。

**例：**当有开始结束点后开始执行协程，每一秒执行一次，路径搜索完成后停止协程。

```c#
private Coroutine findASTAR; //定义协程
void Start(){
	findASTAR = StartCoroutine(ASTARFinding()); //开启协程
}
//协程函数的签名是一个迭代器，事实上我们应该可以通过协程池来控制协程的执行
 IEnumerator ASTARFinding(){
 	  //这里notNull是一个Func委托，返回bool值 我们设计函数当开始结束点不为空时开始执行协程
      yield return new WaitUntil(notNull); 
       //将起始点加入队列
        queue.Enqueue(startPoint.GetComponent<Waypoint>());
        //当队列不为空时开始处理每一个节点
        while (queue.Count > 0)
        {
            //节点出队
            var current_waypoint = queue.Dequeue();
            //若当前节点就是最终的endpoint结束循环
            if (current_waypoint == endPoint.GetComponent<Waypoint>())
            {
                isFinished = true;
                //终止协程
                StopCoroutine(findASTAR);
                break;
            }
            else
            {
                //查找当前节点上下左右的四个节点
                ExploreAround(current_waypoint);
                //将当前节点设置为已访问（isExplored）
                current_waypoint.isExplored = true;
            }
            sortQueue();
            //每0.1秒执行一次协程
            yield return new WaitForSeconds(0.1f);
        }
 
 }
```



## 随机地图

在上述的算法可视化当中，我主要还是利用了较为简单的Random方法来进行随机地图的生成，这样的生成方法会出现两个问题：

- 障碍物的生成坐标完全随机，会出现**重复位置生成**的问题
- 生成的障碍物图会形成闭环，也就是会**出现无法行走的路径**

为了解决上述两种问题，我们在这里引入两种算法：**洗牌算法，洪水填充算法**。

### 洗牌算法

洗牌算法的核心是先通过随机种子得到一个乱序的障碍物生成坐标序列，该序列的全集是整个地图区域，之后我们将该序列存入到一个队列当中，根据队列的FIFO原则，我们每次取得一个随机坐标点后将该点从队列中取出并放到队尾，这样我们便可以完全得到一个不重复的随机队列。

```c#
    /*  洗牌算法
    1. Fisher-Yates Shuffle算法：将每次随机到的数字存储到一个缓存的list中，在下一次随机时删除这些元素并打乱原先的list (缺点：占用了额外的内存空间，空间复杂度高啦)
    2.Knuth-Durstenfeld Shuffle算法：交换元素顺序以及使用队列先进先出的序列进行洗牌（推荐！）
    */
    public static List<Vector2Int> ShuffleCoords(List<Vector2Int> _dataList)
    {
        for (int i = 0; i < _dataList.Count; i++)
        {
            //进阶：这里可以使用噪声作为随机种子，这里是随机在datalist中选择一个数字并与当前交换
            int randomNum = Random.Range(i, _dataList.Count);
            Vector2Int temp = _dataList[randomNum];
            _dataList[randomNum] = _dataList[i];
            _dataList[i] = temp;
        }
        return _dataList;
    }
```

### 洪水填充算法

洪水填充算法的核心其实也是利用了寻路算法，我们利用BFS对整张地图进行搜索并标记当前所有可以行走的区域，其实就是要搜索当前的连通图，我们可以通过access来保存当前可以通行的坐标数量，这里我们用一个类似回溯的算法。我们将某一个随机点加入当前的地图中，使用BFS算法进行计算当前的access值，若地图总量mapSize-access<障碍物总数，表示该点不能再放障碍物了，我们将该随机障碍物点的坐标标记为false，并移除该点。

```c#
private void ObstacleGenerate() {
   int obsCount = (int)(mapSize.x * mapSize.y * obsPercent);
   mapCenter = new Vector2Int((int)(mapSize.x / 2), (int)(mapSize.y / 2));  //设置地图中心点
   mapObstacles = new bool[(int)mapSize.x, (int)mapSize.y];             //初始化二维布尔数组

   //洗牌算法ShuffleCoords，已经得到乱序队列
   TileCoordQueue = new Queue<Vector2Int>(Utility.ShuffleCoords(TileCoordList));

   //默认当前障碍物数量为0
   int currentObsCount = 0;

   for (int i = 0; i < obsCount; i++)
   {
     //根据洗牌算法得到某个可以摆放Obstacle的点
     Vector2Int randomCoord = TileCoordQueue.Dequeue();
     TileCoordQueue.Enqueue(randomCoord);

      //洪水填充算法开始：满足条件才能生成障碍物(类似回溯算法)
     mapObstacles[randomCoord.x, randomCoord.y] = true;     
     currentObsCount++;

     if (randomCoord != mapCenter && MapIsFullyAccessible(mapObstacles, currentObsCount))
     {
			...//添加该障碍物点	
     }
     else {
     	 //移除该障碍物点
         mapObstacles[randomCoord.x, randomCoord.y] = false;
         currentObsCount--;
     }
  }
}

//洪水填充算法
private bool MapIsFullyAccessible(bool[,] _mapObstacles, int _currentObsCount)
{
    bool[,] mapFlags = new bool[_mapObstacles.GetLength(0), _mapObstacles.GetLength(1)]; //这里的mapFlag类似于搜索算法中的visited数组
    Queue<Vector2Int> queue = new Queue<Vector2Int>(); //所有的坐标会在筛选后存储到这个队列中
    queue.Enqueue(mapCenter);
    mapFlags[mapCenter.x, mapCenter.y] = true; //中心点标记为已检测

    int accessibleCount = 1; //初始化当前实际可走的瓦片数量（中心可走，因此现在为1）
    while (queue.Count > 0) {
        Vector2Int currentTile = queue.Dequeue();
        foreach (var item in directions) //按照上右下左的顺序进行遍历搜索
        {
           var explore = currentTile + item;
           int x = explore.x;
           int y = explore.y;
           //判断边界
           if (x >= 0 && x < _mapObstacles.GetLength(0) && y >= 0 && y < 	 mapObstacles.GetLength(1)) {
                if (!mapFlags[x, y] && !_mapObstacles[x, y]) {
                    mapFlags[x, y] = true;
                    accessibleCount++;
                    queue.Enqueue(explore);
                }
            }
        }
    }
    return accessibleCount == (int)(mapSize.x*mapSize.y - _currentObsCount);
}
```



## Navmesh实践

### 静态navmesh生成

通过静态的navmeshSurface对已有的物件根据mesh进行烘焙，通过modifier进行烘焙得到区域。

![image-20210803214552845](image/image-20210803214552845.png)

### 动态navmesh生成

通过添加navmeshObstacle组件对动态生成的障碍物进行实时的烘焙，如图中的cube均由随机算法生成。

![image-20210803214654753](image/image-20210803214654753.png)

