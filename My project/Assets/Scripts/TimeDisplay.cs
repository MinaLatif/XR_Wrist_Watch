using System;
using UnityEngine;
using TMPro;

/// <summary>
/// Displays the current system time and date on TextMeshPro UI elements.
/// Updates periodically for performance optimization.
/// </summary>
public class TimeDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dateText;

    [Header("Format Settings")]
    [Tooltip("Time format string. Default: 12-hour with AM/PM")]
    [SerializeField] private string timeFormat = "h:mm tt";

    [Tooltip("Date format string. Default: Day, Month Date")]
    [SerializeField] private string dateFormat = "ddd, MMM d";

    [Header("Performance")]
    [Tooltip("How often to update the display (in seconds). Lower = more responsive but more CPU usage")]
    [SerializeField] private float updateInterval = 0.5f;

    private float lastUpdateTime;

    private void Start()
    {
        // Update immediately on start
        UpdateDisplay();
    }

    private void Update()
    {
        // Only update at specified interval for performance
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateDisplay();
            lastUpdateTime = Time.time;
        }
    }

    private void UpdateDisplay()
    {
        DateTime now = DateTime.Now;

        if (timeText != null)
        {
            timeText.text = now.ToString(timeFormat);
        }

        if (dateText != null)
        {
            dateText.text = now.ToString(dateFormat);
        }
    }

    /// <summary>
    /// Force an immediate update of the display.
    /// </summary>
    public void ForceUpdate()
    {
        UpdateDisplay();
        lastUpdateTime = Time.time;
    }

    /// <summary>
    /// Set the time format at runtime.
    /// Common formats:
    /// - "h:mm tt" = 1:30 PM (12-hour)
    /// - "hh:mm tt" = 01:30 PM (12-hour with leading zero)
    /// - "H:mm" = 13:30 (24-hour)
    /// - "HH:mm" = 13:30 (24-hour with leading zero)
    /// - "h:mm:ss tt" = 1:30:45 PM (with seconds)
    /// </summary>
    public void SetTimeFormat(string format)
    {
        timeFormat = format;
        ForceUpdate();
    }

    /// <summary>
    /// Set the date format at runtime.
    /// Common formats:
    /// - "ddd, MMM d" = Mon, Jan 19
    /// - "dddd, MMMM d" = Monday, January 19
    /// - "MM/dd/yyyy" = 01/19/2026
    /// - "d MMM yyyy" = 19 Jan 2026
    /// </summary>
    public void SetDateFormat(string format)
    {
        dateFormat = format;
        ForceUpdate();
    }

    /// <summary>
    /// Toggle between 12-hour and 24-hour time format.
    /// </summary>
    public void Toggle24HourFormat(bool use24Hour)
    {
        timeFormat = use24Hour ? "H:mm" : "h:mm tt";
        ForceUpdate();
    }
}
