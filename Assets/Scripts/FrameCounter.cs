using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class FrameCounter : MonoBehaviour {

    private TMP_Text m_Text;
    private Queue<float> m_frameTimes = new();
    float totalTime = 0f;

    [SerializeField]
    [Range(0f, 5f)]
    private float m_movingAverageDuration = 1f;

    [SerializeField]
    private string m_format;

    private string m_prefix;
    private string m_suffix;

    private void Awake() {
        m_Text = GetComponent<TMP_Text>();

        var index = m_format.IndexOf("{0}");

        m_prefix = m_format.Substring(0, index);
        m_suffix = m_format.Substring(index + 3);
    }

    private void Update() {

        float frameRate = 1 / Time.deltaTime;

        if (m_movingAverageDuration > 0f) {

            m_frameTimes.Enqueue(Time.deltaTime);
            totalTime += Time.deltaTime;

            while (totalTime > m_movingAverageDuration) {
                totalTime -= m_frameTimes.Dequeue();
            }

            frameRate = m_frameTimes.Count / totalTime;
        }

        m_Text.text = $"{m_prefix}{Mathf.Round(frameRate).ToString("N0")}{m_suffix}";
    }
}
