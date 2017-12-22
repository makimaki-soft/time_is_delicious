using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class EventCardVM : VMBase {

    private int _temperature;
    public int Temperature
    {
        get { return _temperature; }
        private set
        {
            if (value != _temperature)
            {
                _temperature = value;
                NotifyPropertyChanged();
            }
        }
    }

    private int _humidity;
    public int Humidity
    {
        get { return _humidity; }
        private set
        {
            if (value != _humidity)
            {
                _humidity = value;
                NotifyPropertyChanged();
            }
        }
    }
    private int _wind;
    public int Wind
    {
        get { return _wind; }
        private set
        {
            if (value != _wind)
            {
                _wind = value;
                NotifyPropertyChanged();
            }
        }
    }


    public EventCardVM()
    {
        _temperature = 0;
        _humidity = 0;
        _wind = 0;
        MainModel.Instance.PropertyChanged += Instance_PropertyChanged;
    }

    private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var model = (MainModel)sender;
        switch(e.PropertyName)
        {
            case "CurrentEventCard":
                Temperature = model.CurrentEventCard.Weather[RuleManager.EventType.Temperature];
                Humidity = model.CurrentEventCard.Weather[RuleManager.EventType.Humid];
                Wind = model.CurrentEventCard.Weather[RuleManager.EventType.Wind];
                break;
        }
    }
}
