using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//例子的功能：顾客Order触发点单事件，服务员Calculate响应点单事件并计算账单金额
//事件的拥有者: Customer
//事件：onOrder
//事件的响应者：Waiter类
//事件处理器：Calculate方法
//事件订阅关系：+=

public class EventX : MonoBehaviour
{
    Customer customer = new Customer();
    Customer customer1 = new Customer();
    Waiter waiter = new Waiter();
    private void Start()
    {
        customer.OnOrder += waiter.Calculate;
        customer1.OnOrder += waiter.Calculate;

        customer.OnOrder(customer,new OrderEventArgs());
        customer1.OnOrder(customer, new OrderEventArgs());

        customer.PayTheBill();
        customer1.PayTheBill();
    }
}

public class Customer 
{
    //使用properties属性对数据成员进行包装
    public  float Bill { get; set; }
    public void PayTheBill() {
        Debug.Log("I have to pay " + this.Bill);
    }

    public EventHandler OnOrder;

    public void Order(string _name,string _size,float _price) {
        OrderEventArgs _e = new OrderEventArgs();
        if (OnOrder != null) {
            _e.CoffeeName = _name;
            _e.CoffeeSize = _size;
            _e.CoffeePrice = _price;
        }
        OnOrder(this, _e);
    }
}

//这里就是预制好的事件参数，其中包含咖啡名称，Size大小，以及咖啡价格
public class OrderEventArgs : EventArgs
{
    public string CoffeeName { get; set; }
    public string CoffeeSize { get; set; }
    public float CoffeePrice { get; set; }
}


public class Waiter
{
    //这就是OnOrder事件的handler，服务员类去计算最终价格，参数与事件参数一致
    internal void Calculate(System.Object _customer, EventArgs _args)
    {
        float finalPrice = 0;
        Customer customer = _customer as Customer;
        OrderEventArgs args = _args as OrderEventArgs; 
        switch (args.CoffeeSize)
        {
            case "small":
                finalPrice = args.CoffeePrice;
                break;
            case "mid":
                finalPrice = args.CoffeePrice+3;
                break;
            case "large":
                finalPrice = args.CoffeePrice+6;
                break;
        }
        customer.Bill += finalPrice;
    }
}