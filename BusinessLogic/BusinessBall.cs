//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using BisAPI = TP.ConcurrentProgramming.BusinessLogic.BusinessLogicAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class Ball : IBall
    {
        internal Ball(Data.IBall ball)
        {
            ball.NewPositionNotification += RaisePositionChangeEvent;
            _dataBall = ball;
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall

        #region private    
        private BusinessLogicAbstractAPI _bisLayer = BisAPI.GetBusinessLogicLayer();
        private Data.IBall _dataBall;
        private void RaisePositionChangeEvent(object? sender, Data.Vector e)
        {

            NewPositionNotification?.Invoke(this, new Position(e.x, e.y));

            _bisLayer.CheckCollisions(_dataBall);
        }

        #endregion private
    }
}