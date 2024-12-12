using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Camera cam;
    public Transform alvo;

    Vector2 posicaoInical;

    float zInical;


    Vector2 camDistancia => (Vector2)cam.transform.position - posicaoInical;

    float zDistanciaAlvo => transform.position.z - alvo.transform.position.z;

    float plano => (cam.transform.position.z + (zDistanciaAlvo > 0 ?  cam.farClipPlane : cam.nearClipPlane));

    float fatorParalaxe =>  Mathf.Abs(zDistanciaAlvo / plano);
    // Start is called before the first frame update
    void Start()
    {
        posicaoInical = transform.position;
        zInical = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 novaPosicao = posicaoInical + camDistancia * fatorParalaxe;
        transform.position = new Vector3(novaPosicao.x, novaPosicao.y, zInical);
    }
}
