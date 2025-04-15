//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2023, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//  by introducing yourself and telling us what you do with this community.
//_____________________________________________________________________________________________________________________________________

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TP.ConcurrentProgramming.BusinessLogic;
using LogicIBall = TP.ConcurrentProgramming.BusinessLogic.IBall;
using LogicAPI = TP.ConcurrentProgramming.BusinessLogic.BusinessLogicAbstractAPI;

namespace TP.ConcurrentProgramming.Presentation.Model
{
    internal class ModelBall : IBall
    {
        public ModelBall(double top, double left, LogicIBall underneathBall)
        {
            LogicalTop = top;
            LogicalLeft = left;
            DiameterBackingField = LogicAPI.GetDimensions.BallDimension;
            underneathBall.NewPositionNotification += NewPositionNotification;
        }

        #region IBall

        public double Top => LogicalTop * ScaleHeight;
        public double Left => LogicalLeft * ScaleWidth;
        public double Diameter => DiameterBackingField * ScaleWidth;




        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged

        #endregion IBall

        #region private

        private double LogicalTop;
        private double LogicalLeft;
        private double DiameterBackingField;
        private double ScaleWidth = 1.0;
        private double ScaleHeight = 1.0;
        private void NewPositionNotification(object sender, IPosition e)
        {
            LogicalTop = e.y;
            LogicalLeft = e.x;
            RaisePropertyChanged(nameof(Top));
            RaisePropertyChanged(nameof(Left));
        }

        public void NewScaleNotification(double width, double height)
        {
            ScaleHeight = height;
            ScaleWidth = width;
            RaisePropertyChanged(nameof(Top));
            RaisePropertyChanged(nameof(Left));
            RaisePropertyChanged(nameof(Diameter));
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion private

        #region testing instrumentation

        [Conditional("DEBUG")]
        internal void SetLeft(double x)
        {   LogicalLeft = x;
            RaisePropertyChanged(nameof(Left));
        }

        [Conditional("DEBUG")]
        internal void SettTop(double x)
        { LogicalTop = x;
          RaisePropertyChanged(nameof(Top));
        }

        #endregion testing instrumentation
    }
}