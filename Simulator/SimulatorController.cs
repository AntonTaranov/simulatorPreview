using System;
using System.Collections.Generic;
using BlocksBreaker.Data;
using UnityEngine;

namespace BlocksBreaker.Simulator
{
    public class SimulatorController
    {
        List<BallData> circles;

        bool started = false;

        BallsSpawner ballsSpawner;
        SimulationField simulationField;

        public Action<BallData> SpawnBallCallback;
        public Action<Object2D> KillObjectCallback;
        public Action BallSpawingFinished;
        public Action BonusConsumed;

        public SimulatorController(GameField field)
        {
            circles = new List<BallData>();

            simulationField = new SimulationField(field, 5, 5);
        }

        public void SpawnBalls(uint count, float radius, Vector2 direction, Vector2 startBallPosition, float speedMultiplier)
        {
            ballsSpawner = new BallsSpawner(radius, startBallPosition,
                                                direction, count, 23 * speedMultiplier);
            float delay = 0.05f / (speedMultiplier * 0.8f);
            ballsSpawner.SetDelay(delay);
                    
            started = true;
        }

        public void Update(float deltaTime)
        {
            if (!started) return;

            var colliders = new List<ICollider>(circles);

            simulationField.UpdateStaticColliders();

            var notCollided = CheckCollisions(colliders.ToArray(), deltaTime);
            foreach (var collider in notCollided)
            {
                collider.Update(deltaTime);
            }

            if (ballsSpawner != null)
            {
                var newBalls = ballsSpawner.Update(deltaTime);
                foreach (var ball in newBalls)
                {
                    SpawnBallCallback?.Invoke(ball);
                }
                if (ballsSpawner.IsFinished)
                {
                    ballsSpawner = null;
                    BallSpawingFinished?.Invoke();
                }
                circles.AddRange(newBalls);
            }
        }

        private ICollider[] CheckCollisions(ICollider[] updateColliders, float updateTime, HashSet<ICollider> ignoreArray = null)
        {
            var collisions = simulationField.FindCollisions(updateColliders, updateTime, ignoreArray);
            var notCollided = new HashSet<ICollider>(updateColliders);
            foreach (var collision in collisions)
            {
                var timeLeft = updateTime - collision.Time;
                var toUpdate = ProcessCollision(collision, timeLeft, ignoreArray);
                foreach (var collider in toUpdate)
                {
                    collider.Update(timeLeft);
                }
                if (notCollided.Contains(collision.pair.first.GetUsedCollider()))
                {
                    notCollided.Remove(collision.pair.first.GetUsedCollider());
                }
                if (notCollided.Contains(collision.pair.second.GetUsedCollider()))
                {
                    notCollided.Remove(collision.pair.second.GetUsedCollider());
                }
            }
            var result = new ICollider[notCollided.Count];
            notCollided.CopyTo(result);
            return result;
        }

        private ICollider[] ProcessCollision(Collision collision, float timeLeft, HashSet<ICollider> ignoreArray)
        {
            collision.Process();
            ICollider ignoringCollider = null;
            BallData collider = null;
            if (collision.pair.first is BallData)
            {
                collider = collision.pair.first as BallData;
                ignoringCollider = collision.pair.second.GetUsedCollider();
            }
            if (collision.pair.second is BallData)
            {
                collider = collision.pair.second as BallData;
                ignoringCollider = collision.pair.first.GetUsedCollider();
            }
            if (collider != null && !collider.IsAlive)
            {
                if (circles.Contains(collider))
                {
                    circles.Remove(collider);
                    KillObjectCallback?.Invoke(collider);
                }
            }
            if (ignoringCollider != null)
            {
                if (ignoreArray == null) ignoreArray = new HashSet<ICollider>();
                ignoreArray.Add(ignoringCollider);
                if (ignoringCollider is BonusData)
                {
                    KillObjectCallback?.Invoke(ignoringCollider as BonusData);
                }
            }
            return CheckCollisions(new ICollider[] { collider }, timeLeft, ignoreArray);
        }
    }
}
