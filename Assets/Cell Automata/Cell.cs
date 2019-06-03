using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(Rigidbody))]
public class Cell : MonoBehaviour
{
    public bool bread = false;

    public float saturation;

    public float speed;
    public float size;
    public float viewDistance;

    public float need;

    public enum Mode {hunting, spliting}

    public Mode mode = Mode.hunting;

    void RandomMovement(Rigidbody rb)
    {
        
        Vector3 force = Random.onUnitSphere * speed * rb.mass * 2;
        float invAngleDistance = Vector3.Distance(force.normalized, rb.position.normalized);
        Vector3 centerForce = (force * invAngleDistance)/100;

        rb.AddForce(force + centerForce);
    }


    public void MoveCell(Cell target) 
    {
        Rigidbody rb = this.GetComponent<Rigidbody>();

        if (this.saturation > need*100)
        {
            if (!bread)
            {
                mode = Mode.spliting;
                GetComponent<AnimateCell>().Color2 = Color.magenta * Random.value;
                bread = true;
            }
        }
        else
        {
            mode = Mode.hunting;
            GetComponent<AnimateCell>().Color2 = Color.cyan * Random.value;
        }


        if (target != null) 
        {
            if (mode == Mode.hunting)
            {
                if (target.size < size || target.name == "food") 
                {
                    saturation += target.size;
                    target.gameObject.SetActive(false);
                }
                else 
                {
                    Vector3 force = target.transform.position - this.transform.position;
                    rb.AddForce(force * rb.mass);
                }
            }else if (mode == Mode.spliting && saturation > need *100)
            {
                Population pop = FindObjectOfType<Population>();

                if (Random.Range(0, 100f) < 4)
                {
                    pop.DivideCells(GetComponent<Cell>());
                }
            }
        }
        else 
        {
            RandomMovement(rb);
        }
    }

    public void UpdateCell()
    {
        this.need = (speed + Mathf.Pow(size,2) + viewDistance)/200;

    }

}
