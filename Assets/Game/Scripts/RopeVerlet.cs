using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeVerlet : MonoBehaviour
{
    private List<LineParticle> lineParticles;
    public LineRenderer lineRenderer;

    public float segmentDiff = 0.5f;
    private float lineWidth = 0.01f;

    public Transform anchor;

    public FishingRod fishingRod;
    public GameObject fishingBait;

    private Vector3 currentAcceleration;

    private int minSegmentCount = 2;

    //private bool simulateLineChange = false;
    private void Awake()
    {
        lineParticles = new List<LineParticle>();

        Vector3 startPos = anchor.position;


        lineParticles.Add(new LineParticle(anchor.position, anchor.position));
        lineParticles.Add(new LineParticle(fishingBait.transform.position, fishingBait.transform.position));

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;


        lineRenderer.SetPositions(new[] { anchor.position, fishingBait.transform.position });


    }

    private void Start()
    {
        //EventManager.instance.AddListener(EventEnums.onPlayerFishingBaitThrow, (Hashtable hashtable) => { simulateLineChange = true; });
        //EventManager.instance.AddListener(EventEnums.onBaitReachTarget, (Hashtable hashtable) => { simulateLineChange = false;  });
        //EventManager.instance.AddListener(EventEnums.onPlayerFishingBaitReel, (Hashtable hashtable) => { simulateLineChange = true; });
    }

    private void DrawRope()
    {
        lineRenderer.positionCount = lineParticles.Count;
        for (int i = 0; i < lineParticles.Count; i++)
        {
            lineRenderer.SetPosition(i, lineParticles[i].posNow);
        }
    }

    private void Simulate()
    {

        SimulateOnLineChanged();
        SimulateGravity();            

        for (int i = 0; i < 50; i++)
        {
            ApplyConstraint();
        }
    }

    private void SimulateOnLineChanged()
    {
        var delta = Vector3.Distance(anchor.position, fishingBait.transform.position);
        var targetSegLength = Mathf.RoundToInt(delta / segmentDiff) + 1;
        var oldLength = lineRenderer.positionCount;
        
        var deltaLength = targetSegLength - oldLength;

        if (targetSegLength <= minSegmentCount)
        {
            //set positions to 2, from rod to bait
            lineRenderer.positionCount = minSegmentCount;

            if (lineParticles.Count > minSegmentCount)
            {
                lineParticles.RemoveRange(minSegmentCount, lineParticles.Count - minSegmentCount);
            }
        }
        else
        {

            lineRenderer.positionCount = lineRenderer.positionCount + deltaLength;


            if (lineParticles.Count < lineRenderer.positionCount)
            {
                var count = lineRenderer.positionCount - lineParticles.Count;
                for (int i = 0; i < count; i++)
                {
                    LineParticle newParticle;
                    var index = lineParticles.Count - 1 - i;

                    newParticle = new LineParticle(lineParticles[index].posNow, fishingBait.transform.position);

                    lineParticles.Add(newParticle);
                }
            }
            else if (lineParticles.Count > lineRenderer.positionCount)
            {
                var from = Mathf.Max(lineRenderer.positionCount,2);
                var count = lineParticles.Count - lineRenderer.positionCount;
                lineParticles.RemoveRange(from, count);
                
            }
        }
        
    }


    private void SimulateGravity()
    {
        Vector3 forceGravity = new Vector3(0, -1, 0);

        for (int i = 1; i < lineParticles.Count - 1; i++)
        {
            var segment = lineParticles[i];
            Vector3 velocity = Vector3.zero;

            velocity = segment.posNow - segment.posOld;
            segment.posOld = segment.posNow;
            segment.posNow += velocity;
            segment.posNow += forceGravity * Time.fixedDeltaTime;


        }

    }

    private void ApplyConstraint()
    {
        if (lineParticles.Count > 0)
        {
            var firstSegment = lineParticles[0];
            firstSegment.posNow = anchor.position;

            lineParticles[lineParticles.Count - 1].posNow = fishingBait.transform.position;
        }
        

        for (int i = 0; i < lineParticles.Count - 1; i++)
        {
            var firstSeg = this.lineParticles[i];
            var secondSeg = this.lineParticles[i + 1];

            float dist = Vector3.Distance(firstSeg.posNow, secondSeg.posNow);
            float error = dist - segmentDiff;


            Vector3 changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            Vector3 changeAmount = changeDir * error;

            if (i == 0)
            {
                secondSeg.posNow += changeAmount;
            }
            else if (i + 1 == lineParticles.Count - 1)
            {
                firstSeg.posNow -= changeAmount;
            }
            else
            {
                firstSeg.posNow -= changeAmount * 0.5f;
                secondSeg.posNow += changeAmount * 0.5f;
            }

        }

    }



    // Update is called once per frame
    void Update()
    {
        //for(int i = 0; i < lineParticles.Count; i++)
        //{
        //    Verlet(lineParticles[i], Time.fixedDeltaTime);


        //}
        DrawRope();

    }

    private void FixedUpdate()
    {
        Simulate();
    }


    //private void Verlet(LineParticle p, float dt)
    //{
    //    var temp = p.Pos;
    //    p.Pos += p.Pos - p.OldPos + (p.Acceleration * dt * dt);
    //    p.OldPos = temp;
    //}

    //private void PoleConstraint(LineParticle p1, LineParticle p2, float restLength)
    //{
    //    var delta = p2.Pos - p1.Pos;

    //    var deltaLength = delta.magnitude;

    //    var diff = (deltaLength - restLength) / deltaLength;

    //    p1.Pos += delta * diff * 0.5f;
    //    p2.Pos -= delta * diff * 0.5f;
    //}
}

public class LineParticle
{

    public Vector3 posOld;
    public Vector3 posNow;

    public LineParticle(Vector3 posOld, Vector3 posNow)
    {
        this.posOld = posOld;
        this.posNow = posNow;
    }

}