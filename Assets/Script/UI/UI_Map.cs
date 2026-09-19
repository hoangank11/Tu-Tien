using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Map : MonoBehaviour
{
    [System.Serializable]
    public class MapPoint
    {
        [Tooltip("Chỉ để hiển thị cho dễ nhận biết trong Inspector, không dùng để so sánh logic.")]
        public string pointName;

        [Tooltip("Object mốc tương ứng, kéo từ UI_Canvas/UI_Map/ArrowParent (vd: object tên '1', '2'...).")]
        public GameObject arrowObject;

        [Tooltip("Tất cả các scene thuộc điểm mốc này. Nhiều scene có thể dùng chung 1 điểm mốc.")]
        public List<string> sceneNames = new List<string>();
    }

    [Header("Danh sách toàn bộ điểm mốc trên bản đồ")]
    [Tooltip("Kéo object mốc (trong ArrowParent) + khai báo tên các scene tương ứng cho từng điểm mốc. " +
             "Khi có scene mới, chỉ cần thêm tên scene vào đúng điểm mốc (hoặc thêm điểm mốc mới nếu có object mũi tên mới).")]
    [SerializeField]
    private List<MapPoint> mapPoints = new List<MapPoint>()
    {
        new MapPoint { pointName = "Mốc 1" },
        new MapPoint { pointName = "Mốc 2" },
        new MapPoint { pointName = "Mốc 3" },
        new MapPoint { pointName = "Mốc 4" },
        new MapPoint { pointName = "Mốc 5" },
        new MapPoint { pointName = "Mốc 6" },
        new MapPoint { pointName = "Mốc 7" },
        new MapPoint { pointName = "Mốc 8" },
        new MapPoint { pointName = "Mốc 9" },
        new MapPoint { pointName = "Mốc 10" },
    };

    private Dictionary<string, MapPoint> sceneToPointLookup;

    private void Awake()
    {
        BuildLookup();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        RefreshMapPoint();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshMapPoint();
    }

    private void BuildLookup()
    {
        sceneToPointLookup = new Dictionary<string, MapPoint>();

        foreach (var point in mapPoints)
        {
            if (point == null || point.sceneNames == null)
                continue;

            foreach (var sceneName in point.sceneNames)
            {
                if (string.IsNullOrEmpty(sceneName))
                    continue;

                if (!sceneToPointLookup.ContainsKey(sceneName))
                    sceneToPointLookup.Add(sceneName, point);
            }
        }
    }

    public void RefreshMapPoint()
    {
        if (sceneToPointLookup == null)
            BuildLookup();

        string currentScene = SceneManager.GetActiveScene().name;
        sceneToPointLookup.TryGetValue(currentScene, out MapPoint activePoint);

        foreach (var point in mapPoints)
        {
            if (point == null || point.arrowObject == null)
                continue;

            point.arrowObject.SetActive(point == activePoint);
        }
    }
    public void OnClickClose()
    {
        UI.instance.CloseMapUI();
    }
}