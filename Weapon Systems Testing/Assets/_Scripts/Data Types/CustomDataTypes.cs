using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public delegate void TimerEnd();
    TimerEnd endCallback;

    private float current;
    private float max;

    private bool timerRunning = false;

    public Timer(float max, TimerEnd endCallback)
    {
        this.max = max;
        current = max;

        timerRunning = false;

        this.endCallback = endCallback;
    }

    /// <summary>
    /// Increments the timer by dt and checks to see if the current time is less than 0, if so, call the callback function
    /// </summary>
    /// <param name="dt">The time in seconds that the timer should be incremented by</param>
    public void Increment(float dt)
    {
        if (timerRunning)
        {
            current -= dt;

            if (current <= 0)
            {
                Stop();

                // Calls the registered callback function
                endCallback();
            }
        }
    }

    #region Timer Function Methods

    /// <summary>
    /// Starts the timer from the max time, effectively restarting it.
    /// </summary>
    public void Start()
    {
        current = max;
        timerRunning = true;
    }
    /// <summary>
    /// Stops the timer and sets the current time to 0.
    /// </summary>
    public void Stop()
    {
        timerRunning = false;
        current = 0;
    }
    /// <summary>
    /// Pauses the timer at the current time that it has recorded, can be resumed later using Start.
    /// </summary>
    public void Pause()
    {
        timerRunning = false;
    }
    /// <summary>
    /// Resumes the timer at the current time that it has recorded.
    /// </summary>
    public void Resume()
    {
        timerRunning =true;
    }
    /// <summary>
    /// Resets the current time to be equal to the max time, does not start the timer.
    /// </summary>
    public void Reset()
    {
        current = max;
    }

    #endregion

    public float GetTime()
    {
        return current;
    }
    public void SetTime(float time)
    {
        current = time;
    }
    public void SetMaxTime(float max)
    {
        this.max = max;
        current = max;
    }

}

public class TimerManager 
{ 
    public Dictionary<string, Timer> timers { get; private set; } = new Dictionary<string, Timer>();

    public void IncrementTimers(float dt)
    {
        // Runs the increment function on each timer
        foreach (Timer timer in timers.Values)
            timer.Increment(dt);
    }

    public void Add(string name, Timer timer)
    {
        timers.Add(name, timer);
    }
    public void Remove(string name)
    {
        timers.Remove(name);
    }
    public bool ContainsKey(string name)
    {
        return timers.ContainsKey(name);
    }
}

