using UnityEngine;
public class Particle : MonoBehaviour{
    float a, x, y;
    float angle;
    [SerializeField] SpriteRenderer sprite;
    Color color;
    void Set(Transform source, float power, Color color){
        this.color = color;
        sprite.color = color;
        transform.SetParent(source);
        a = power + Random.Range(0.5f, 1f);
        float r = Random.Range(0.25f, 1f);
        angle = Random.Range(-Mathf.PI, Mathf.PI);
        x = source.transform.position.x + r * Mathf.Sin(angle);
        y = source.transform.position.y + 1f + r * Mathf.Cos(angle);
        transform.position = new Vector3(x, y, 0);
        transform.localScale = new Vector3(a, a, 1);
        sprite.sortingOrder = -(int)transform.position.y * 1000 + Random.Range(-10, 10);
    }

    void Update(){
        a -= Time.deltaTime * Core.TimeScale();
        float dx = a * Time.deltaTime * Mathf.Sin(angle);
        float dy = a * Time.deltaTime * Mathf.Cos(angle);
        x += dx;
        y += dy;
        sprite.color= new Color(color.r, color.g, color.b, a);
        if(a < 0){
            DestroyImmediate(gameObject);
            DestroyImmediate(this);
            return;
        }
        sprite.transform.Rotate(0, 0, 60f * Time.deltaTime * Core.TimeScale());
        transform.position = new Vector3(x, y, 0);
        transform.localScale = new Vector3(a, a, 1);

    }

    public static void Spawn(Transform source, float power, Color color, string type="Flake"){
        Instantiate(Resources.Load<Particle>("Particles/"+type)).Set(source, power, color);
    }
}