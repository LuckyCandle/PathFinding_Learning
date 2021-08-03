using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���ӵĹ��ܣ��˿�Order�����㵥�¼�������ԱCalculate��Ӧ�㵥�¼��������˵����
//�¼���ӵ����: Customer
//�¼���onOrder
//�¼�����Ӧ�ߣ�Waiter��
//�¼���������Calculate����
//�¼����Ĺ�ϵ��+=

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
    //ʹ��properties���Զ����ݳ�Ա���а�װ
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

//�������Ԥ�ƺõ��¼����������а����������ƣ�Size��С���Լ����ȼ۸�
public class OrderEventArgs : EventArgs
{
    public string CoffeeName { get; set; }
    public string CoffeeSize { get; set; }
    public float CoffeePrice { get; set; }
}


public class Waiter
{
    //�����OnOrder�¼���handler������Ա��ȥ�������ռ۸񣬲������¼�����һ��
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