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
            LogicalDiameter = LogicAPI.GetDimensions.BallDimension;
            underneathBall.NewPositionNotification += NewPositionNotification;
        }

        #region IBall

        public double Top => LogicalTop * Scale;
        public double Left => LogicalLeft * Scale;
        public double Diameter => LogicalDiameter * Scale;




        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged

        #endregion IBall

        #region private

        private double LogicalTop;
        private double LogicalLeft;
        private double LogicalDiameter;
        private double Scale = 1.0;
      
        private void NewPositionNotification(object sender, IPosition e)
        {
            LogicalTop = e.y;
            LogicalLeft = e.x;
            RaisePropertyChanged(nameof(Top));
            RaisePropertyChanged(nameof(Left));
        }

        public void NewScaleNotification(double scale)
        {
            Scale = scale;
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