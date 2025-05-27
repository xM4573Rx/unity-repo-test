using UnityEngine;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections.Concurrent;
using TMPro;

public class ObjectController : MonoBehaviour
{
    public GameObject infoPanel; // Asigna este desde el editor de Unity

    public GameObject targetObject;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI labelText;

    private TcpListener listener;
    private Thread listenerThread;
    private ConcurrentQueue<string> commandQueue = new ConcurrentQueue<string>();

    private float simulatedTemperature = 20f; // Comienza en 20°C
    private bool increasing = true; // Controla si sube o baja



    void Start()
    {
        listenerThread = new Thread(StartServer);
        listenerThread.IsBackground = true;
        listenerThread.Start();
    }

    void StartServer()
    {
        listener = new TcpListener(IPAddress.Loopback, 5500);
        listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        listener.Start();

        while (true)
        {
            var client = listener.AcceptTcpClient();
            using var reader = new StreamReader(client.GetStream());
            var message = reader.ReadLine();
            if (!string.IsNullOrEmpty(message))
            {
                commandQueue.Enqueue(message);
            }
        }
    }

    void Update()
    {
        // Manejo desde teclado (en Unity directamente)
        float moveSpeed = 5f;
        float rotSpeed = 100f;

        if (Input.GetKey(KeyCode.W))
            targetObject.transform.position += Vector3.forward * moveSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.S))
            targetObject.transform.position += Vector3.back * moveSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
            targetObject.transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.D))
            targetObject.transform.position += Vector3.right * moveSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftArrow))
            targetObject.transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.RightArrow))
            targetObject.transform.Rotate(Vector3.up, -rotSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.UpArrow))
            targetObject.transform.Rotate(Vector3.right, rotSpeed * Time.deltaTime);
       
        if (Input.GetKey(KeyCode.DownArrow))
            targetObject.transform.Rotate(Vector3.right, -rotSpeed * Time.deltaTime);

        //if (Input.GetKey(KeyCode.Space))
        //    targetObject.transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        //if (Input.GetKey(KeyCode.LeftControl))
        //    targetObject.transform.position += Vector3.down * moveSpeed * Time.deltaTime;

        while (commandQueue.TryDequeue(out string cmd))
        {
            HandleCommand(cmd);
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            simulatedTemperature += 20f * Time.deltaTime; // Ajusta la velocidad si quieres
            simulatedTemperature = Mathf.Clamp(simulatedTemperature, 0f, 100f);
            targetObject.transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            simulatedTemperature -= 20f * Time.deltaTime;
            simulatedTemperature = Mathf.Clamp(simulatedTemperature, 0f, 100f);
            targetObject.transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        }

        // Convertir temperatura en color: de azul (0°C) a rojo (100°C)
        float t = Mathf.InverseLerp(20f, 100f, simulatedTemperature);
        float r = t;
        float g = 1f - t;
        float b = 0f;

        Color newColor = new Color(r, g, b);

        // Aplicar color al objeto
        Renderer rend = targetObject.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = newColor;
        }

        // Mostrar temperatura en texto
        if (statusText != null)
        {
            string label = simulatedTemperature < 33 ? "baja" :
                           simulatedTemperature > 66 ? "alta" : "media";
            statusText.text = $"{simulatedTemperature:F0}°C";
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (infoPanel != null)
            {
                infoPanel.SetActive(!infoPanel.activeSelf);
            }
        }
    }

    void HandleCommand(string cmd)
    {
        switch (cmd)
        {
            case "MOVE":
                targetObject.transform.position += new Vector3(1, 0, 0);
                statusText.text = "Movimiento: →";
                break;
            case "ROTATE":
                targetObject.transform.Rotate(0, 45, 0);

                break;
            case "COLOR":
                // Aumentar o disminuir temperatura
                if (increasing)
                {
                    simulatedTemperature += 5f;
                    if (simulatedTemperature >= 100f)
                    {
                        simulatedTemperature = 100f;
                        increasing = false;
                    }
                }
                else
                {
                    simulatedTemperature -= 5f;
                    if (simulatedTemperature <= 0f)
                    {
                        simulatedTemperature = 0f;
                        increasing = true;
                    }
                }

                // Convertir temperatura en color: de azul (0°C) a rojo (100°C)
                float t = Mathf.InverseLerp(0f, 100f, simulatedTemperature); // 0 a 1
                float r = t;
                float g = 0f;
                float b = 1f - t;

                Color newColor = new Color(r, g, b);

                // Aplicar color al objeto
                Renderer rend = targetObject.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material.color = newColor;
                }

                // Mostrar temperatura en texto
                if (statusText != null)
                {
                    string label = simulatedTemperature < 33 ? "baja" :
                                   simulatedTemperature > 66 ? "alta" : "media";

                    //statusText.text = $"Temperatura {label}: {simulatedTemperature:F0}°C";
                    statusText.text = $"Temperatura:\n{simulatedTemperature:F0}°C";
                }
                break;
            case "TOGGLE_PANEL":
                if (infoPanel != null)
                {
                    infoPanel.SetActive(!infoPanel.activeSelf); // Alterna visibilidad
                }
                break;
        }
    }

    void OnApplicationQuit()
    {
        listener?.Stop();
        listenerThread?.Abort();
    }
}
