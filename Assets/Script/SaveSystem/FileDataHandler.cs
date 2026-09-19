using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class FileDataHandler
{
    private string fullPath;
    private bool encryptData;
    private string codeWorld = "Noname100";

    public FileDataHandler(string dataDirPath, string dataFileName, bool encryptData)
    {
        fullPath = Path.Combine(dataDirPath, dataFileName);
        this.encryptData = encryptData;
    }

    public void SaveData(GameData gameData)
    {
        try
        {
            // 1. tạo một directory nếu nó chưa tồn tại
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // 2. chuyển đổi gamedata thành string JSON
            string dataToSave = JsonUtility.ToJson(gameData, true);

            if (encryptData)
                dataToSave = EncryptDecrypt(dataToSave);

            // 3. mở/tạo file mới
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                // 4. viết JSON text  vào file 
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToSave);
                }
            }
        }

        catch (Exception e)
        {
            // debug các lỗi diễn ra
            Debug.LogError("Lỗi khi cố gắng save file data: " + fullPath + "\n" + e);
        }
    }


    public GameData LoadData()
    {
        GameData loadData = null;
        // 1. check xem file đó có tồn tại hay không?
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                // 2. mở file
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    // 3. đọc file text contents
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if(encryptData)
                    dataToLoad = EncryptDecrypt(dataToLoad);

                // 4. chuyển đổi ngược string JSON thành gamedata để sử dụng
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {

                // debug các lỗi hiện tại
                Debug.LogError("Lỗi load file game: " + fullPath + "\n" + e);
            }
        }
        return loadData;
    }

    public void Delete()
    {
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    private string EncryptDecrypt(string data)
    {
        string modifedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifedData += (char)(data[i] ^ codeWorld[i % codeWorld.Length]);
        }
        return modifedData;
    }

}
