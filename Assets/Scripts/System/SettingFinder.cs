using UnityEngine;

public class SettingFinder
{
    public static int GetSettingScore()
    {
        int performanceScore = 0;

        bool lowCap = false;

        // RAM (peso alto)
        if (SystemInfo.systemMemorySize >= 6000) performanceScore += 3;
        else if (SystemInfo.systemMemorySize >= 4000) performanceScore += 2;
        else if (SystemInfo.systemMemorySize >= 3000) performanceScore += 1;
        else lowCap = true; // RAM muito baixa → força Low

        Debug.Log("RAM: " + SystemInfo.systemMemorySize);

        // GPU Memory (peso alto)
        if (SystemInfo.graphicsMemorySize >= 3000) performanceScore += 3;
        else if (SystemInfo.graphicsMemorySize >= 1500) performanceScore += 2;
        else if (SystemInfo.graphicsMemorySize >= 700) performanceScore += 1;
        else lowCap = true; // GPU fraca → força Low

        Debug.Log("GPU Mem: " + SystemInfo.graphicsMemorySize);

        // GPU Shader Level (peso médio)
        if (SystemInfo.graphicsShaderLevel >= 60) performanceScore += 2;
        else if (SystemInfo.graphicsShaderLevel >= 50) performanceScore += 1;

        Debug.Log("Shader: " + SystemInfo.graphicsShaderLevel);

        // CPU Cores (peso baixo)
        if (SystemInfo.processorCount >= 8) performanceScore += 1;
        else if (SystemInfo.processorCount >= 4) performanceScore += 0; // não soma nada, mas não é ruim

        Debug.Log("Cores: " + SystemInfo.processorCount);

        // CPU Frequency (peso baixo)
        if (SystemInfo.processorFrequency >= 2200) performanceScore += 1;
        else if (SystemInfo.processorFrequency >= 1500) performanceScore += 0;

        Debug.Log("CPU MHz: " + SystemInfo.processorFrequency);

        if (lowCap) performanceScore = 0;
        
        Debug.Log("Performance Score: " + performanceScore);
        
        return performanceScore;
    }
}
