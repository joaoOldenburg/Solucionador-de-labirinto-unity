using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;



public class parede : MonoBehaviour
{
    Thread mThread;
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;
    IPAddress localAdd;
    TcpListener listener2;
    TcpClient client;

    public Texture cubeTexture;
    Vector3 cubeScale = new Vector3(1, 1, 1);
    bool running;
    int[,] matriz;
    int[,] matrix;

    // Preencher a matriz com valores aleatórios de 1 e 0



    void montarCenario()
    {
        // Percorrer a matriz e criar um cubo onde o valor for 0
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (matrix[i, j] == 0)
                {
                    // Calcular a posição do cubo na matriz
                    Vector3 receivedPos = new Vector3(i, 1, j);

                    // Criar um novo cubo com a escala e posição especificadas
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.localScale = cubeScale;
                    cube.transform.position = receivedPos;

                    // Criar um novo material e atribuir a textura especificada
                    Material cubeMaterial = new Material(Shader.Find("Standard"));
                    cubeMaterial.mainTexture = cubeTexture;

                    // Atribuir o material ao cubo
                    Renderer cubeRenderer = cube.GetComponent<Renderer>();
                    cubeRenderer.material = cubeMaterial;
                }
            }
        }
    }
    void Start()
    {
        GetInfo();

        montarCenario();
    }

    void GetInfo()
    {
        localAdd = IPAddress.Parse(connectionIP);
        listener2 = new TcpListener(IPAddress.Any, connectionPort);
        listener2.Start();

        client = listener2.AcceptTcpClient();

        running = true;
        while (running)
        {
            SendAndReceiveData();
        }
        listener2.Stop();

    }

    void SendAndReceiveData()
    {
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];

        //---receiving Data from the Host----
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize); //Getting data in Bytes from Python
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead); //Converting byte data to string

        if (dataReceived != null)
        {
            //---Using received data---
            StringToVector3(dataReceived);

        }
    }

    public void StringToVector3(string sVector)
    {

        int x;
        int y;
        
        if (sVector.Split(',').Length == 2)
        {
            string[] dimensao;
            dimensao = sVector.Split(',');
            x = Int32.Parse(dimensao[0]);
            y = Int32.Parse(dimensao[1]);

        matrix = new int[x, y];
        return;
        }


        // split the items
        string[] sArray = sVector.Split(',');

        int i = 0;
        foreach (var item in sArray)
        {

            var item2 = item.Replace("[", "").Replace("]", "");
            string[] dadosmatrix = item2.Split(' ');
            for (int j = 0; j < dadosmatrix.Length; j++)
            {
                if (dadosmatrix[j] == "0")
                    //matrix[i, j] = int.Parse(dadosmatrix[i]);
                    matrix[i, j] = 0;
                else
                    matrix[i, j] = 1;
                //if (dadosmatrix[j].Contains("0"))
                    //matrix[i, j] = int.Parse(dadosmatrix[i]);
                    //matrix[i, j] = 0;
                //else
                    //matrix[i, j] = 1;
            }
            i++;
        }
        running = false;

        
    }
}