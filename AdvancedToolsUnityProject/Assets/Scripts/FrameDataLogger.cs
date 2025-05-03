using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Globalization;
using UnityEngine.SceneManagement;

public class FrameDataLogger : MonoBehaviour
{
    bool flushed = false;
    private string filePath;
    private StringBuilder logBuffer;

    float TimeOffset = 0;

    public int incemental = 100;
    public int limit = 10000;
    public int startAmount = 0;

    public string fileName = "Statistics_IndividualObjects";

    void Start()
    {
        if (ObjectSpawner.spawnCount == 0)
            ObjectSpawner.spawnCount = startAmount;

        TimeOffset = Time.time;

        filePath = Path.Combine(Application.dataPath, "Data", $"{fileName}_{ObjectSpawner.spawnCount}.csv");

        logBuffer = new StringBuilder(1048576);
        logBuffer.AppendLine("TotalFrame,Time,DeltaTime,FPS");
    }

    void Update()
    {
        float time = Time.time - TimeOffset;
        if (time > 10)
        {
            FlushBufferToDisk();

            if (ObjectSpawner.spawnCount >= limit)
                return;

            ObjectSpawner.spawnCount += incemental;
            SceneManager.LoadScene(0);
        }
        else
        {
            float currentTime = time;
            float deltaTime = Time.deltaTime;
            float fps = 1f / deltaTime;

            logBuffer.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0},{1:F4},{2:F6},{3:F2}", Time.frameCount, currentTime, deltaTime, fps));

        }
    }

    void OnDestroy()
    {
        FlushBufferToDisk();
    }

    void OnApplicationQuit()
    {
        FlushBufferToDisk();
    }

    private void FlushBufferToDisk()
    {
        if (flushed)
            return;
        flushed = true;
        try
        {
            File.AppendAllText(filePath, logBuffer.ToString());
            logBuffer.Clear();
        }
        catch (IOException ex)
        {
            Debug.LogError($"Failed to write frame data: {ex.Message}");
        }
    }
}
