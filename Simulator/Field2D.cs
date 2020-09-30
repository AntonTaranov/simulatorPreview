using System.Collections.Generic;
using UnityEngine;
namespace BlocksBreaker.Simulator
{
    public class Field2D
    {
        readonly Cell[] cells;

        ICollider[] staticColliders;

        public Field2D(float x, float y, float width, float height, int numRows, int numColumns)
        {
            cells = new Cell[numRows * numColumns];
            var cellWidth = width / numColumns;
            var cellHeight = height / numRows;

            var bottomWall = new FinishLIne(new Vector2(x, y), new Vector2(x + width, y));
            var rightWall = new LineData(new Vector2(x + width, y), new Vector2(x + width, y + height));
            var topWall = new LineData(new Vector2(x + width, y + height), new Vector2(x, y + height));
            var leftWall = new LineData(new Vector2(x, y + height), new Vector2(x, y));

            for (int column = 0; column < numColumns; column++)
            {
                for(int row = 0; row < numRows; row++)
                {
                    var cell = new Cell(x + cellWidth * column, y + cellHeight * row,
                                        cellWidth, cellHeight);
                    cells[column + row * numColumns] = cell;

                    if (column == 0)
                    {
                        cell.Colliders.Add(leftWall);
                    }
                    if (column == numColumns - 1)
                    {
                        cell.Colliders.Add(rightWall);
                    }

                    if (row == 0)
                    {
                        cell.Colliders.Add(bottomWall);
                    }
                    if (row == numRows - 1)
                    {
                        cell.Colliders.Add(topWall);
                    }
                }
            }
        }

        public void SetStaticColliders(ICollider[] colliders)
        {
            staticColliders = colliders;
        }

        public Collision[] FindCollisions(ICollider[] collidersToUpdate, float updateTime, HashSet<ICollider> ignoreArray)
        {
            var collisions = new List<Collision>();
            var collidersInCell = new List<ICollider>();
            foreach (Cell cell in cells)
            {
                collidersInCell.Clear();
                foreach(var collider in collidersToUpdate)
                {
                    if (cell.IsInside(collider, updateTime))
                    {
                        collidersInCell.Add(collider);
                    }
                }
                if (collidersInCell.Count > 0)
                {
                    foreach(var collider in staticColliders)
                    {
                        if (cell.IsInside(collider,updateTime))
                        {
                            collidersInCell.Add(collider);
                        }
                    }
                    collidersInCell.AddRange(cell.Colliders);
                    if (ignoreArray != null)
                    {
                        collidersInCell.RemoveAll((ICollider obj) => ignoreArray.Contains(obj));
                    }
                    for (int i = 0; i < collidersInCell.Count; i++)
                    {
                        var colliderOne = collidersInCell[i];
                        Collision firstCollision = null;
                        for (int j = i + 1; j < collidersInCell.Count; j++)
                        {
                            var colliderTwo = collidersInCell[j];
                            if (colliderOne.CanCollideWith(colliderTwo))
                            {
                                var collision = colliderOne.GetCollision(colliderTwo, updateTime);
                                if (collision != null)
                                {
                                    if (firstCollision == null || firstCollision.Time > collision.Time)
                                    {
                                        firstCollision = collision;
                                    }
                                }
                            }
                        }
                        if (firstCollision != null)
                        {
                            //check if collision already in results
                            bool alreadyInResult = false;
                            foreach (var collision in collisions)
                            {
                                var pair = collision.pair;
                                if (pair.first == firstCollision.pair.first && pair.second == firstCollision.pair.second
                                    || pair.first == firstCollision.pair.second && pair.second == firstCollision.pair.first)
                                {
                                    alreadyInResult = true;
                                    break;
                                }
                            }
                            if (!alreadyInResult)
                            {
                                collisions.Add(firstCollision);
                            }
                        }
                    }
                }
            }

            return collisions.ToArray();
        }

    }
}
