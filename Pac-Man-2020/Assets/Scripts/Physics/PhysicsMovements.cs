using UnityEngine;
using System.Collections.Generic;


public class Movements
{
    private bool inIntersection;
    private float magnitude = 0f;
    private Vector2 velocity = new Vector2(0f,0f);
    private Vector2 direction = new Vector2(0f,0f);
    private Vector2 requestedDirection = new Vector2(0f, 1f);
    private bool[] validDirections = { true, true, true, false}; // +x -x +y -y

    public Movements(){}

    public Movements(Vector2 direction, float magnitude){
        this.direction = direction;
        this.magnitude = magnitude;
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
    public void SetValidDirections(bool[] directions){
        validDirections = directions;
    }

    public void Turn(){
        direction = requestedDirection;
    }

    public void SetDirection(Vector2 direction){
        if(inIntersection){
            if(!direction.Equals(new Vector2(0f,0f)) && validMove(direction)){
                requestedDirection = direction;
            }
        }else{
            if(!direction.Equals(new Vector2(0f,0f)) && validMove(direction)){
                this.direction = direction;
                velocity = magnitude * direction;
            }
        }
    }

    public void SetRandomDirection(Vector2 positionOfCharacter, Vector2 position){
        Vector2 newDirection;
        do{
            newDirection = GetRandomVector();
        }while(!validMove(newDirection));

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
    public void SetInIntersection(bool inIntersection){
        this.inIntersection = inIntersection;
    }

    // used to prevent turning into the wall in a gutter
    private bool validMove(Vector2 direction) 
    {
        int index = DirectionToIndex(direction); //index of 'validDirectons' array corresponding to direction
        
        return validDirections[index];
    }
    // on exit intersection make sure the character can only move forward and back, but can't turn into the wall
    public void ExitIntersection(){
        if(direction.x == 0f){
            SetValidDirections( new bool[4]{false, false, true , true});
        }else{
            SetValidDirections( new bool[4]{true , true, false, false});
        }
    }

    // update valid directions based on the specific intersection
    public void EnterIntersection(Collider2D col){
        int pillID = col.transform.parent.gameObject.GetInstanceID(); 
        Dictionary<int, bool[]> intersections = new ValidDirections().getIntersections();
        validDirections = intersections[pillID];
    }

    // determines whether the requested direction is valid at the specific intersection
    // public bool validTurn(Vector2 direction, int intersectionId){
    //     Dictionary<int, bool[]> intersections = new ValidDirections().getIntersections();
    //     int index;

    //     // validDirections[i] i==0 -> +x ; i==1 -> -x ; i==2 -> +y ; i== -> -y;
    //     if(direction.x == 1f){
    //         index = 0;
    //     }else if(direction.x == -1f){
    //         index = 1;
    //     }else if(direction.y == 1f){
    //         index = 2;
    //     }else if(direction.y == -1f){
    //         index = 3;
    //     }else{
    //         return false; // in case direction is a null vector
    //     }

    //     return intersections[intersectionId][index];    // returns a bool from the intersections dictionary in the specific direction
    // }

    // Trabslate direction to an index that corresponds to the slot of the direction in the 'validDirections' array
    private int DirectionToIndex(Vector2 direction){
        if(direction.x == 1f){
            return 0;
        }else if(direction.x == -1f){
            return 1;
        }else if(direction.y == 1f){
            return 2;
        }else if(direction.y == -1f){
            return 3;
        }else{
            return -1; // in case direction is a null vector
        }
    }
}

