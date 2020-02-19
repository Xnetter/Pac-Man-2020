using UnityEngine;

enum Directions{ x1 = 1, x2 = -1, y1 = 1, y2 = -1};

public class Movements
{
    private Vector2 direction = new Vector2(0f,0f);
    private float magnitude = 0f;
    private Vector2 velocity = new Vector2(0f,0f);
    private GameObject maze;
    private bool[] validDirections = {true, true, true, false}; // x1 x2 y1 y2

    public Movements(){}

    public Movements(Vector2 direction, float magnitude, GameObject maze){
        this.direction = direction;
        this.magnitude = magnitude;
        this.maze = maze;
        velocity = direction * magnitude;
    }

    public Vector2 GetDirection(){
        return direction;
    }
    public float GetMagnitude(){
        return magnitude;
    }
    public Vector2 GetVelocity(){
        return velocity;
    }
    public GameObject GetMazeOb(){
        return maze;
    }
    public void SetMazeOb(GameObject maze){
        this.maze = maze;
    }
    public void SetValidDirections(bool[] directions){
        validDirections = directions;
    }

    public void SetDirection(Vector2 direction, Vector2 position, float charRadius){
        if(!direction.Equals(new Vector2(0f,0f)) && CanMove(direction, position, charRadius)){
            this.direction = direction;
            velocity = magnitude * direction;
        }
    }

    public void SetRandomDirection(Vector2 positionOfCharacter, Vector2 position, float charRadius){
        Vector2 newDirection;
        do{
            newDirection = GetRandomVector();
        }while(!CanMove(newDirection, positionOfCharacter, charRadius));

        direction = newDirection;
        velocity = newDirection * magnitude;
    }

    private Vector2 GetRandomVector(){
         switch((int)Random.Range(0, 4)){   // returns a number between 0 and 4 inclusive
                case 0:
                    return new Vector2(-1,0);
                case 1:
                    return new Vector2(1, 0);
                case 2:
                    return new Vector2(0, -1);
                case 3:
                    return new Vector2(0, 1);
                default:
                    return new Vector2(0, 1);   // in case newDirection is 4 
            }
    }

    public void SetMagnitude(float magnitude){
        this.magnitude = magnitude;
        velocity = magnitude * direction;
    }


    private bool CanMove(Vector2 direction, Vector2 position, float charRadius)    // Raycast to detect collisions between pac-man and environment.
    {
        // 4 positions on the perimeter of the collider
        Vector2 positionX1 = new Vector2(position.x + charRadius, position.y);
        Vector2 positionX2 = new Vector2(position.x - charRadius, position.y);
        Vector2 positionY1 = new Vector2(position.x, position.y + charRadius);
        Vector2 positionY2 = new Vector2(position.x, position.y - charRadius);

        RaycastHit2D hitX1 = Physics2D.Linecast(positionX1 + direction, positionX1);
        RaycastHit2D hitX2 = Physics2D.Linecast(positionX2 + direction, positionX2);
        RaycastHit2D hitY1 = Physics2D.Linecast(positionY1 + direction, positionY1);
        RaycastHit2D hitY2 = Physics2D.Linecast(positionY2 + direction, positionY2);
        
        // hit.collider won't be null because the ray always passes through the character's collider 
        if(false){

        }else{
            Debug.Log("Pos1" + hitX1.collider.ToString());
            Debug.Log("Pos2" + hitX2.collider.ToString());
            Debug.Log("Pos3" + hitY1.collider.ToString());
            Debug.Log("Pos4" + hitY2.collider.ToString());
            return true;
        }
        
        
    }
    void onCollisionEnter(){
        Debug.Log("collision");
    }

}

