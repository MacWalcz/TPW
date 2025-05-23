﻿//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace TP.ConcurrentProgramming.Presentation.Model
{
    public interface IBall : INotifyPropertyChanged
    {
        double Top { get; }
        double Left { get; }
        double Diameter { get; }

        void NewScaleNotification(double scale);

    }

    

    public abstract class ModelAbstractApi : IObservable<IBall>, IDisposable
    {
        public static ModelAbstractApi CreateModel()
        {
            return modelInstance.Value;
        }

        public abstract void OnWindowSizeChanged(double width, double height);

        public abstract double Scale { get; set; }

        public abstract event EventHandler<BallChaneEventArgs> BallChanged;

        public abstract double BoardWidth { get; }

        public abstract double BoardHeight { get; } 
        public abstract void Start(int numberOfBalls);
        #region IObservable

        public abstract IDisposable Subscribe(IObserver<IBall> observer);

        #endregion IObservable

        #region IDisposable

        public abstract void Dispose();

        #endregion IDisposable

        #region private

        private static Lazy<ModelAbstractApi> modelInstance = new Lazy<ModelAbstractApi>(() => new ModelImplementation());

        public abstract class BallChaneEventArgs : EventArgs
        {
            public abstract IBall Ball { get; init; }
        }

        #endregion private
    }
}