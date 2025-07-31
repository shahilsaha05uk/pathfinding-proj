using UnityEngine;
using UnityEngine.UI;

public class ObstaclePanel : MonoBehaviour
{
    [SerializeField] private Controller Controller;

    public Button btnUpdateObstacleDensity;
    public Button btnClearObstacles;
    public Button btn10Obstacles;
    public Button btn20Obstacles;
    public Button btn30Obstacles;

    public PanelSlider sliderObstacleDensity;

    void Start()
    {
        btnUpdateObstacleDensity.onClick.AddListener(OnUpdateObstacleDensityButtonClick);
        btnClearObstacles.onClick.AddListener(OnClearObstaclesButtonClick);
        
        btn10Obstacles.onClick.AddListener(() => OnDensityPercentButtonClick(0.1f));
        btn20Obstacles.onClick.AddListener(() => OnDensityPercentButtonClick(0.2f));
        btn30Obstacles.onClick.AddListener(() => OnDensityPercentButtonClick(0.3f));
    }

    public float GetObstacleDensity() => sliderObstacleDensity.GetValue();

    private void OnDensityPercentButtonClick(float density)
    {
        sliderObstacleDensity.UpdateValue(density);
    }

    private void OnUpdateObstacleDensityButtonClick()
    {
        Controller.OnUpdateObstacleDensity(sliderObstacleDensity.GetValue());
    }

    private void OnClearObstaclesButtonClick() => Controller.OnClearObstacleDensity();
}
