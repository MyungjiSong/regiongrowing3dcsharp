using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEditor.VersionControl;
using UnityEngine.UIElements;


public class RegionGrowingManager : MonoBehaviour
{
    public void RegionGrowing()
    {
        if (!flag)
        {
            List<ushort> ushortList = new List<ushort>();
            flag = false;
            int width = openCvManager.width; // 3d volume width size
            int height = openCvManager.height; // 3d volume height size
            int depth = openCvManager.depth; // 3d volume depth size

            ushort[] ushorts = new ushort[512 * 512 * 145]; // final 3d segmented volume data
            regionGrowing = new ushort[512, 512, 145]; // copying space to store in-range data
            Array.Clear(regionGrowing, 0, 512*512*145); // initialize

            RegionGrowing((int)intersect.x, (int)intersect.y, (int)intersect.z);
        }
    }

    public void RegionGrowing(int iniX, int iniY, int iniZ)
    {
        int[] posItem = new[] { iniX, iniY, iniZ };
        int xv, yv, zv;
        int i, j, k;
        int[] newItem = new int[3];
        
        QNode currFront;
        Queue q = new Queue();     
        q.enqueue(posItem);

        int count = 0;
        while (q.front != null)
        {
            currFront = q.front;
            xv = currFront.dataOne;
            yv = currFront.dataTwo;
            zv = currFront.dataThree;
            //print(xv + ", " + yv + ", " + zv);
            q.dequeue();
            for (i = -1; i <= 1; i++) {
                for (j = -1; j <= 1; j++) {
                    for (k = -1; k <= 1; k++) {
                        if ((xv + i >= 0) && (xv + i <= width) &&
                            (yv + j >= 0) && (yv + j <= height) &&
                            (zv + k >= 0) && (zv + k <= depth) &&
                            !((i == 0) && (j == 0) && (k == 0)) &&
                            (regionGrowing[xv + i, yv + j, zv + k] == 0) &&
                            (GetVal(xv + i, yv + j, zv + k) <= (pivot + range)) &&
                            (GetVal(xv + i, yv + j, zv + k) >= (pivot - range)))
                        {
                            count++;
                            //print("It works!" + counts + "i: " + i + "j: " + j + "k: " + k);
                            SetVal(xv + i, yv + j, zv + k);
                            newItem[0] = xv + i;
                            newItem[1] = yv + j;
                            newItem[2] = zv + k;
                            q.enqueue(newItem);
                        }
                        else
                        {
                            //print("Fail to add!!!" + " intensity : " + GetVal(xv + i, yv + j, zv + k) + ", RegionGrowingvalue: " + "\ni: " + i + "j: " + j + "k: " + k);
                        }
                    }
                }
            }
        }
        print("Total is : " + count);
    }


    class Queue
    {
        public QNode front, rear;

        public Queue()
        {
            this.front = this.rear = null;
        }

        public void enqueue(int[] item)
        {
            QNode temp = new QNode(item);

            if (this.rear == null) {
                this.front = this.rear = temp;
                return;
            }
            else
            {
                this.rear.next = temp;
                this.rear = this.rear.next;
            }
        }

        public void dequeue()
        {
            if (this.front != null)
            {
                this.front = this.front.next;

                if (this.front == null)
                {
                    this.rear = null;
                    print("All finished");
                }
            }
        }
    }


    class QNode
    {
        public int dataOne; // x
        public int dataTwo; // y
        public int dataThree; // z
        public QNode next;

        public QNode(int[] item)
        {
            this.dataOne = item[0];
            this.dataTwo = item[1];
            this.dataThree = item[2];
            this.next = null;
        }
    }


    // allData is my own 3d volume listed to 1d array
    ushort GetVal(int x, int y, int z)
    {
        return (ushort)allData[x + 512 * y + 512 * 512 * z].w; // intensity value of x,y,z position
    }


    void SetVal(int x, int y, int z)
    {
        regionGrowing[x, y, z] = (ushort)allData[x + 512 * y + 512 * 512 * z].w;
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
