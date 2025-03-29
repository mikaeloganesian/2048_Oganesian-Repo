using UnityEngine;
using TMPro;
using UnityEngine.UI; 
using System;

public class Cell : MonoBehaviour
{
    public event Action<int> OnValueChanged; // отслеживание нового значения
    public event Action<int, int> OnPositionChanged;
    
    public int X { get; private set; }
    public int Y { get; private set; }
    
    public int Value { get; private set; }
    
    public bool isEmpty => Value == 0;

    
    public int Points => isEmpty ? 0 : (int)Mathf.Pow(2, Value);
    
    public const int maxValue = 11;

    [SerializeField] private TextMeshProUGUI points;
    [SerializeField] private Image cellImage;

    public void SetValue(int x, int y, int value)
    {
        bool positionChanged = (this.X != x) || (this.Y != y);
        bool valueChanged = this.Value != value;

        this.X = x;
        this.Y = y;
        this.Value = value;

        if (positionChanged)
        {
            OnPositionChanged?.Invoke(x, y);
        }
        if (valueChanged)
        {
            OnValueChanged?.Invoke(value);
        }


        UpdateCell();
    }

    public void UpdateCell()
    {
        UpdateCellColor();
        points.text = isEmpty ? string.Empty : Points.ToString();    
    }
    
    private void UpdateCellColor()
    {
        if (isEmpty)
        {
            SetColor(Color.gray); 
        }
        else
        {
            Color cellColor = GetColorByValue(Value);
            SetColor(cellColor);
        }
    }

    private Color GetColorByValue(int value)
    {
        switch (value)
        {
            case 1: return new Color(0.9f, 0.9f, 0.9f); 
            case 2: return new Color(0.8f, 0.9f, 1.0f); 
            case 3: return new Color(0.6f, 0.8f, 1.0f);
            case 4: return new Color(0.4f, 0.7f, 1.0f); 
            case 5: return new Color(0.2f, 0.6f, 1.0f); 
			case 6: return new Color(0.6f, 1.0f, 0.2f);
			case 7: return new Color(1.0f, 0.6f, 0.2f);
			case 8: return new Color(0.6f, 0.2f, 1.0f);
			case 9: return new Color(1.0f, 0.2f, 0.6f);
			case 10: return new Color(0.7f, 1.0f, 0.4f);
			case 11: return new Color(0.6f, 0.3f, 0.9f);
            default: return Color.white; 
        }
    }

    private void SetColor(Color color)
    {
		if (cellImage != null) 
        	{
            	cellImage.color = color;
        	}
    }
}
