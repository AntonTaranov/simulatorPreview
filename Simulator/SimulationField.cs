using System;
using System.Collections.Generic;
using BlocksBreaker.Data;

namespace BlocksBreaker.Simulator
{
    public class SimulationField
    {
        GameField gameField;

        Field2D fieldWithCells;

        public SimulationField(GameField field, int numRows, int numColumns)
        {
            var halfWidth = field.Width * 0.5f;
            var halfHeight = field.Height * 0.5f;

            gameField = field;

            fieldWithCells = new Field2D(-halfWidth, -halfHeight, field.Width, field.Height, numRows, numColumns);
        }

        public void UpdateStaticColliders()
        {
            var staticColliders = new List<ICollider>();
            gameField.staticObjects.ForEach((Object2D obj) => { if (obj is ICollider) { staticColliders.Add(obj as ICollider); } });
            fieldWithCells.SetStaticColliders(staticColliders.ToArray());
        }

        public Collision[] FindCollisions(ICollider[] colliders, float deltaTime, HashSet<ICollider> ignoreColliders)
        {
            return fieldWithCells.FindCollisions(colliders, deltaTime, ignoreColliders);
        }
    }
}
