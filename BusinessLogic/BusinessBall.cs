//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________
using System.Numerics;
using BisAPI = TP.ConcurrentProgramming.BusinessLogic.BusinessLogicAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class Ball : IBall
{
    public Ball(Data.IBall ball)
    {
        ball.NewPositionNotification += RaisePositionChangeEvent;
    }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

    #endregion IBall

    #region private

    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {

        NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
        if (sender is Data.IBall ball)
        {
            if (BoardContact(e))
            {
                    ball.Contact();    
            }
        }

    }

    private bool BoardContact(Data.IVector e)
    {
        double rightBoundary = BisAPI.GetDimensions.TableWidth - BisAPI.GetDimensions.BallDimension;
        double bottomBoundary = BisAPI.GetDimensions.TableHeight - BisAPI.GetDimensions.BallDimension;

        if (e.x >= rightBoundary || e.y >= bottomBoundary || e.x <= 0 || e.y <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }


    }

    #endregion private
}
}