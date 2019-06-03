using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Population : MonoBehaviour
{
    List<Cell> cellPopulation = new List<Cell>();
    public int population = 10;
    public int currentLiving = 1;

    public int food = 10;
    public int foodLeft;

    public float avgSize;
    public float avgViewDistance;
    public float avgSpeed;
    public float avgNeed;
    public float avgSaturation;

    public float foodinSeconds = 30;

    public Material foodMaterial;
    public GameObject foodPrefab;

    public Material cellMaterial;
    public GameObject cellPrefab;



    void InitializePopulation(int count, List<Cell> cellPopulation)
    {
        cellPopulation.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject cellGameObject = Instantiate(cellPrefab);
            cellGameObject.GetComponent<Renderer>().material = cellMaterial;
            cellGameObject.transform.SetParent(this.transform);
            cellGameObject.transform.SetPositionAndRotation(Random.insideUnitSphere*population,Quaternion.identity);
            cellGameObject.name = "cell";

            cellGameObject.AddComponent<Cell>();
            cellGameObject.GetComponent<Rigidbody>().useGravity = false;
            cellGameObject.GetComponent<Rigidbody>().drag = 2;
            cellGameObject.GetComponent<Rigidbody>().angularDrag = 2;

            Cell cell = cellGameObject.GetComponent<Cell>();

            cell.size = Random.Range(10f,50f);
            cell.speed = Random.Range(1f, 50f);
            cell.viewDistance = Random.Range(1f, 50f);
            cell.saturation = 20;
            cell.transform.localScale = new Vector3(1, 1, 1) * cell.size / 5;

            cell.UpdateCell();

            cellPopulation.Add(cell);

            AnimateCell anim = cellGameObject.AddComponent<AnimateCell>();
            anim.Color1 = (Color.cyan / 2) * Random.value;
            anim.Color2 = (Color.cyan) * Random.value;

            //anim.Color1 = (Random.ColorHSV() / 2) * Random.value;
            //anim.Color2 = Random.ColorHSV() * Random.value;

        }
    }

    private IEnumerator UpdateCellLife(List<Cell> cellPopulation)
    {
        while (true)
        {


            for (int i = 0; i < cellPopulation.Count; i++) 
            {
                Cell cell = cellPopulation[i];

                if (cell.name == "cell" && cell.gameObject.activeSelf == true)
                {
                    for (int j = 0; j < cellPopulation.Count; j++) 
                    {
                        Cell target = cellPopulation[j];

                        if (cell != target && target != null) 
                        {
                            if (Vector3.Distance(target.transform.position, cell.transform.position) > cell.viewDistance || target.gameObject.activeSelf == false) 
                            {
                                target = null;
                            }
                        }

                        cell.saturation -= cell.need/cellPopulation.Count;
                        cell.MoveCell(target);
                    }

                    if (cell.saturation < 0) 
                    {
                        cell.gameObject.SetActive(false);
                    }

                    yield return new WaitForSecondsRealtime(0.00001f);
                }
            }
            yield return new WaitForSeconds(.1f);


            currentLiving = 0;
            foodLeft = 0;
            float totalSize = 0;
            float totalVIewDistance = 0;
            float totalSpeed = 0;
            float totalNeed = 0;
            float totalSaturation = 0;


            for (int i = 0; i < cellPopulation.Count; i++)
            {
                if (cellPopulation[i].name == "cell" && cellPopulation[i].gameObject.activeSelf == true) 
                {
                    currentLiving++;

                    totalSize += cellPopulation[i].size;
                    totalSpeed += cellPopulation[i].speed;
                    totalVIewDistance += cellPopulation[i].viewDistance;
                    totalNeed += cellPopulation[i].need;
                    totalSaturation += cellPopulation[i].saturation;

                    cellPopulation[i].bread = false;
                }

                if (cellPopulation[i].name == "food" && cellPopulation[i].gameObject.activeSelf == true) 
                {
                    foodLeft++;
                }

                if (cellPopulation[i].isActiveAndEnabled == false) 
                {
                    Destroy(cellPopulation[i].gameObject);
                    cellPopulation.Remove(cellPopulation[i]);
                    i--;

                }
            }

            avgSize = totalSize / currentLiving;
            avgSpeed = totalSpeed / currentLiving;
            avgViewDistance = totalVIewDistance / currentLiving;
            avgNeed = totalNeed / currentLiving;
            avgSaturation = totalSaturation / currentLiving;

            
        }
    }

    public void BreedCells(Cell cell1, Cell cell2)
    {
        GameObject newCellGameObject = Instantiate(cellPrefab);
        newCellGameObject.GetComponent<Renderer>().material = cellMaterial;
        newCellGameObject.transform.SetParent(this.transform);
        newCellGameObject.transform.SetPositionAndRotation(Random.insideUnitSphere * population, Quaternion.identity);
        newCellGameObject.name = "cell";
        
        newCellGameObject.AddComponent<Cell>();
        newCellGameObject.GetComponent<Rigidbody>().useGravity = false;
        newCellGameObject.GetComponent<Rigidbody>().drag = 2;

        Cell newCell = newCellGameObject.GetComponent<Cell>();

        newCell.saturation = (cell1.saturation / 2) + (cell2.saturation / 2);
        cell1.saturation -= cell1.saturation / 2;
        cell2.saturation -= cell2.saturation / 2;

        newCell.size = (cell1.size + cell2.size) / 2;
        //newCell.size += Random.Range(0, avgSize / 2);

        newCell.speed = (cell1.speed + cell2.speed) / 2;
        //newCell.speed += Random.Range(0, avgSpeed / 2);

        newCell.viewDistance = (cell1.viewDistance + cell2.viewDistance) / 2;
        //newCell.viewDistance += Random.Range(-avgViewDistance / 2, avgViewDistance / 2);

        newCell.UpdateCell();
        newCellGameObject.transform.localScale = new Vector3(1, 1, 1) * newCell.size / 5;

        cellPopulation.Add(newCell);

        //Debug.Log("mate");

        AnimateCell anim = newCellGameObject.AddComponent<AnimateCell>();
        AnimateCell animParent1 = cell1.GetComponent<AnimateCell>();
        AnimateCell animParent2 = cell2.GetComponent<AnimateCell>();

        anim.Color1 = (animParent1.Color1 + animParent2.Color1) / 2;
        anim.Color2 = (animParent1.Color2 + animParent2.Color2) / 2;
    }

    IEnumerator AddFood(int count)
    {
        while (true)
        {
            
            for (int i = 0; i < count - foodLeft; i++) 
            {
                GameObject cellGameObject = Instantiate(foodPrefab);
                cellGameObject.GetComponent<Renderer>().material = foodMaterial;
                cellGameObject.transform.SetParent(this.transform);
                cellGameObject.transform.SetPositionAndRotation(Random.insideUnitSphere * Mathf.Pow(i, 1f / 2) * 10, Quaternion.identity);
                cellGameObject.name = "food";

                cellGameObject.AddComponent<Cell>();
                cellGameObject.GetComponent<Rigidbody>().useGravity = false;
                cellGameObject.GetComponent<Rigidbody>().drag = 2;

                Cell cell = cellGameObject.GetComponent<Cell>();

                cell.size = Random.Range(5f, 10f);
                cell.speed = 0;
                cell.viewDistance = 0;

                cellPopulation.Add(cell);

                cellGameObject.AddComponent<AnimateCell>();
            }

            yield return new WaitForSeconds(foodinSeconds);
        }
    }

    public void DivideCells(Cell cell)
    {
        //splits into two equal cells

        GameObject cellGameObject = Instantiate(cellPrefab);
        cellGameObject.GetComponent<Renderer>().material = cellMaterial;
        cellGameObject.transform.SetParent(this.transform);
        cellGameObject.transform.SetPositionAndRotation(cell.transform.position + (Random.insideUnitSphere * 10), Quaternion.identity);
        cellGameObject.name = "cell";

        cellGameObject.AddComponent<Cell>();
        cellGameObject.GetComponent<Rigidbody>().useGravity = false;
        cellGameObject.GetComponent<Rigidbody>().drag = 2;
        cellGameObject.GetComponent<Rigidbody>().angularDrag = 2;

        Cell childCell = cellGameObject.GetComponent<Cell>();

        childCell.size = cell.size / 2;
        childCell.speed = cell.speed / 2;
        childCell.viewDistance = cell.viewDistance / 2;
        childCell.saturation = cell.saturation / 2;
        childCell.transform.localScale = new Vector3(1, 1, 1) * cell.size / 5;

        cell.saturation -= cell.saturation / 2;

        childCell.UpdateCell();

        cellPopulation.Add(childCell);

        AnimateCell anim = cellGameObject.AddComponent<AnimateCell>();
        anim.Color1 = (Color.cyan / 2) * Random.value;
        anim.Color2 = (Color.cyan) * Random.value;
    }


    void Start()
    {
        InitializePopulation(population, cellPopulation);

        StartCoroutine(AddFood(food));

        StartCoroutine(UpdateCellLife(cellPopulation));
    }
}
