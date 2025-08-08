using UnityEngine;
class BodyFrame{
    public int count { get; private set; }
    Vector2 [] positions;
    float [] rotations;
    public BodyFrame(int count){
        this.count = count;
        positions = new Vector2[count];
        rotations = new float[count];
    }
    public void Set(int i, Vector2 position, float rotations){
        positions[i] = position;
        this.rotations[i] = rotations;
    }

    public BodyFrame MoveTo(BodyFrame next, float delta){
        BodyFrame mid = new BodyFrame(count);
        for(int i = 0; i < count; i++){
            mid.positions[i] = positions[i] * (1f - delta) + next.positions[i] * delta;
            mid.rotations[i] = rotations[i] * (1f - delta) + next.rotations[i] * delta;
        }
        return mid;
    }

    public Vector2 GetPosition(int i){  return positions[i];    }
    public float GetRotation(int i){  return rotations[i];    }
    public override string ToString(){
        string text = count.ToString();
        for(int i = 0; i < count; i++)
            text += "|" + positions[i].x.ToString() + ";" + positions[i].y.ToString() + ";"
                + rotations[i].ToString();
        return text;
    }
    public static BodyFrame Parse(int count, string text){
        string [] lines = text.Split('|');
        BodyFrame frame = new BodyFrame(count);
        for(int i = 0; i < count; i++){
            string [] keys = lines[i].Split(';');
            frame.Set(i, new Vector2(float.Parse(keys[0]), float.Parse(keys[1])),
                        float.Parse(keys[2]));
        }
        return frame;
    }

}