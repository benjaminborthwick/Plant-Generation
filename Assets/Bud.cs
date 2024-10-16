using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bud {
    Vector3 currTrunkCenter;
    Color barkColor, leafColor;
    int order, budLength, numOffshootAngles, numOffshoots;
    float branchRadius, offshootDisposition, tendencyToDirection, growthModifier;
    Vector3 tan, norm, binorm;
    List<Mesh> internodeMeshes, leafMeshes;
    static float leafWidth = 3;
    static float leafLength = 9;
    static float lengthRatio = 0.3f;

    public Bud(Vector3 initPos, int order) {
        internodeMeshes = new List<Mesh>();
        leafMeshes = new List<Mesh>();
        currTrunkCenter = initPos;
        budLength = 0;
        this.order = order;
        barkColor = new Color(0.7f + 0.1f * UnityEngine.Random.value, 0.45f + 0.2f * UnityEngine.Random.value, 0.24f + 0.2f * UnityEngine.Random.value, 1);
        leafColor = new Color(0.2f + 0.4f * UnityEngine.Random.value, 0.4f + 0.3f * UnityEngine.Random.value, 0.2f + 0.2f * UnityEngine.Random.value, 1);
        if (order == 0) {
            branchRadius = 5 + 5 * UnityEngine.Random.value;
            tan = new Vector3(0, 1, 0);
            norm = new Vector3(1, 0, 0);
            binorm = new Vector3(0, 0, 1);
            numOffshootAngles = 2 + (int) (4 * UnityEngine.Random.value);
            numOffshoots = 1 + (int) (3 * UnityEngine.Random.value);
            offshootDisposition = UnityEngine.Random.value * 1.5f - 0.5f;
            tendencyToDirection = UnityEngine.Random.value * 0.3f;
            growthModifier = UnityEngine.Random.value * 0.1f;
        }
    }

    public Bud(Vector3 initPos, int order, float prevBranchRadius, Vector3 prevTan, Vector3 prevNorm, 
            Vector3 prevBinorm, int numOffshootAngles, int numOffshoots, float offshootDisposition, 
            Color barkColor, Color leafColor, float tendencyToDirection, float growthModifier) : this(initPos, order) {
        branchRadius = (0.7f + 0.3f * UnityEngine.Random.value) * prevBranchRadius;
        this.tan = prevTan;
        this.norm = prevNorm;
        this.binorm = prevBinorm;
        Vector3.OrthoNormalize(ref tan, ref norm, ref binorm);
        this.numOffshootAngles = numOffshootAngles;
        this.numOffshoots = numOffshoots;
        this.offshootDisposition = offshootDisposition;
        this.barkColor = barkColor;
        this.leafColor = leafColor;
        this.tendencyToDirection = tendencyToDirection;
    }

    public void update(List<Bud> budsToCull, List<Bud> budsToAdd) {
        if (order == 0) {
            if (UnityEngine.Random.value < 0.5 * (budLength / branchRadius - 10)) budsToCull.Add(this);
            else if (UnityEngine.Random.value > 0.15 + 0.03 * budLength){
                internodeMeshes.Add(branchTurn());
                internodeMeshes.Add(branchInternode(9 + UnityEngine.Random.value * 2));
                if (Mathf.Sqrt(UnityEngine.Random.value) < 0.25 + growthModifier + 0.03 * (++budLength - branchRadius)) {
                    generateOffshoots(budsToAdd);
                }
            }
        } else if (order == 1) {
            if (UnityEngine.Random.value < 0.03 * (budLength - 5)) budsToCull.Add(this);
            else if (UnityEngine.Random.value > 0.1 + 0.03 * budLength){
                internodeMeshes.Add(branchTurn());
                internodeMeshes.Add(branchInternode(7 + UnityEngine.Random.value * 2));
                generateLeaves();
                if (Mathf.Sqrt(UnityEngine.Random.value) < 0.2 + growthModifier + 0.03 * (++budLength - branchRadius)) {
                    generateOffshoots(budsToAdd);
                }
            }
        } else if (order == 2) {
            if (UnityEngine.Random.value < 0.02 * (budLength - 3)) budsToCull.Add(this);
            else if (UnityEngine.Random.value > 0.1 + 0.04 * budLength){
                internodeMeshes.Add(branchTurn());
                internodeMeshes.Add(branchInternode(5 + UnityEngine.Random.value * 1));
                generateLeaves();
                if (Mathf.Sqrt(UnityEngine.Random.value) < 0.25 + growthModifier + 0.02 * (++budLength - branchRadius)) {
                    generateOffshoots(budsToAdd);
                }
            }
        } else if (order == 3) {
            if (UnityEngine.Random.value < 0.02 * (budLength - 2)) budsToCull.Add(this);
            else if (UnityEngine.Random.value > 0.1 + 0.04 * budLength / branchRadius){
                internodeMeshes.Add(branchTurn());
                internodeMeshes.Add(branchInternode(3 + UnityEngine.Random.value * 1));
                generateLeaves();
                budLength++;
                /*if (Mathf.Sqrt(UnityEngine.Random.value) < 0.36 + 0.04 * budLength) {
                    for (int i = 0; i < numOffshoots; i++) {
                        float offShootAngle = Mathf.Deg2Rad * (i * (360 / numOffshoots) + UnityEngine.Random.value * 20 - 10 + (360 / numOffshootAngles) * (budLength % numOffshootAngles));
                        Vector3 newTan = (offShootDisposition - 0.2 + 0.4 * UnityEngine.Random.value) * tan + norm * Mathf.Cos(offShootAngle) + binorm * Mathf.Sin(offShootAngle);
                        buds.Add(new Bud(currTrunkCenter, order + 1, branchRadius, newTan, norm, binorm, numOffshootAngles, numOffshoots, offshootDisposition));
                    }
                }*/
            }
        }
    }

    void generateOffshoots(List<Bud> budsToAdd) {
        for (int i = 0; i < numOffshoots; i++) {
            float offShootAngle = Mathf.Deg2Rad * (i * (360 / numOffshoots) + UnityEngine.Random.value * 40 - 10 + (360 / numOffshootAngles) * (budLength % numOffshootAngles));
            Vector3 newTan = (offshootDisposition - 0.2f + 0.4f * UnityEngine.Random.value) * tan + norm * Mathf.Cos(offShootAngle) + binorm * Mathf.Sin(offShootAngle);
            budsToAdd.Add(new Bud(currTrunkCenter, order + 1, branchRadius, newTan, norm, binorm, numOffshootAngles, numOffshoots, offshootDisposition, barkColor, leafColor, tendencyToDirection, growthModifier));
        }
    }

    void generateLeaves() {
        Vector3 genPoint = currTrunkCenter - tan;
        for (int i = 0; i < numOffshoots; i++) {
            float leafLength = Bud.leafLength - 2 + UnityEngine.Random.value * 4;
            float leafWidth = Bud.leafWidth - 0.5f + UnityEngine.Random.value;
            float lengthRatio = Bud.lengthRatio - 0.25f + UnityEngine.Random.value * 0.5f;
            float leafAngle = Mathf.Deg2Rad * (i * (360 / numOffshoots) + UnityEngine.Random.value * 40 - 10 + (360 / numOffshootAngles) * (budLength % numOffshootAngles));
            Vector3 newTan = ((offshootDisposition - 0.1f + 0.2f * UnityEngine.Random.value) * tan + norm * Mathf.Cos(leafAngle) + binorm * Mathf.Sin(leafAngle)).normalized;
            Vector3[] verts = new Vector3[9];
            verts[0] = genPoint + tan * 0.2f;
            verts[1] = genPoint + tan * 0.2f + newTan * branchRadius * 3;
            verts[2] = genPoint - tan * 0.2f + newTan * branchRadius * 3;
            verts[3] = genPoint - tan * 0.2f;
            verts[4] = genPoint + newTan * branchRadius * 2.9f;
            verts[5] = genPoint + newTan * branchRadius * (2.9f + leafLength * lengthRatio);
            verts[6] = genPoint + newTan * branchRadius * (2.9f + leafLength);
            verts[7] = genPoint + newTan * branchRadius * (2.9f + leafLength * lengthRatio) + leafWidth * tan;
            verts[8] = genPoint + newTan * branchRadius * (2.9f + leafLength * lengthRatio) - leafWidth * tan;
            int[] tris = {0, 1, 2, 0, 2, 3, 0, 2, 1, 0, 3, 2, 4, 5, 7, 4, 5, 8, 4, 7, 5, 4, 8, 5, 5, 6, 7, 5, 6, 8, 5, 7, 6, 5, 8, 6};
            Mesh leaf = new Mesh();
            leaf.vertices = verts;
            leaf.triangles = tris;
            leafMeshes.Add(leaf);
        }
    }

    public void Unalive(List<Bud> buds) {
        buds.Remove(this);
        GameObject branch = new GameObject("Branch");
        branch.AddComponent<MeshFilter>();
        branch.AddComponent<MeshRenderer>();
        branch.GetComponent<MeshFilter>().mesh = assembleMeshes();
        branch.GetComponent<Renderer>().material.color = barkColor;
        GameObject leaves = new GameObject("Leaves");
        leaves.AddComponent<MeshFilter>();
        leaves.AddComponent<MeshRenderer>();
        leaves.GetComponent<MeshFilter>().mesh = assembleLeaves();
        leaves.GetComponent<Renderer>().material.color = leafColor;
    }
    
    Mesh branchTurn() {
        Vector3[] verts = new Vector3[40];
        for (int i = 0; i < 20; i++) {
            verts[i] = currTrunkCenter + branchRadius * (norm * Mathf.Cos(Mathf.Deg2Rad * i * 18) + binorm * Mathf.Sin(Mathf.Deg2Rad * i * 18));
        }
        float offsetAngle = UnityEngine.Random.value * 360;
        Vector3 newTan = tan + 0.2f * UnityEngine.Random.value * (norm * Mathf.Cos(Mathf.Deg2Rad * offsetAngle) + binorm * Mathf.Sin(Mathf.Deg2Rad * offsetAngle));
        if (order <= 1) newTan += tendencyToDirection * Vector3.up;
        else newTan += tendencyToDirection * Vector3.ProjectOnPlane(newTan, Vector3.up).normalized;
        float turnAngle = Vector3.Angle(tan, newTan);
        tan = newTan;
        Vector3.OrthoNormalize(ref tan, ref norm, ref binorm);
        currTrunkCenter = currTrunkCenter + tan * branchRadius * Mathf.Sin(Mathf.Deg2Rad * turnAngle);
        for (int i = 0; i < 20; i++) {
            verts[20 + i] = currTrunkCenter + branchRadius * (norm * Mathf.Cos(Mathf.Deg2Rad * i * 18) + binorm * Mathf.Sin(Mathf.Deg2Rad * i * 18));
        }
        int[] tris = new int[120];
        for (int i = 0; i < 20; i++) {
            tris[i * 6] = i;
            tris[i * 6 + 1] = 20 + i;
            tris[i * 6 + 2] = i == 19 ? 20 : 21 + i;
            tris[i * 6 + 3] = i;
            tris[i * 6 + 4] = i == 19 ? 20 : 21 + i;
            tris[i * 6 + 5] = i == 19 ? 0 : i + 1;
        }
        Mesh joint = new Mesh();
        joint.vertices = verts;
        joint.triangles = tris;
        return joint;
    }

    Mesh branchInternode(float segLength) {
        Vector3[] verts = new Vector3[40];
        for (int i = 0; i < 20; i++) {
            verts[i] = currTrunkCenter + branchRadius * (norm * Mathf.Cos(Mathf.Deg2Rad * i * 18) + binorm * Mathf.Sin(Mathf.Deg2Rad * i * 18));
        }
        currTrunkCenter = currTrunkCenter + tan * segLength;
        branchRadius = (0.85f + 0.1f * UnityEngine.Random.value) * branchRadius;
        for (int i = 0; i < 20; i++) {
            verts[20 + i] = currTrunkCenter + branchRadius * (norm * Mathf.Cos(Mathf.Deg2Rad * i * 18) + binorm * Mathf.Sin(Mathf.Deg2Rad * i * 18));
        }
        int[] tris = new int[120];
        for (int i = 0; i < 20; i++) {
            tris[i * 6] = i;
            tris[i * 6 + 1] = 20 + i;
            tris[i * 6 + 2] = i == 19 ? 20 : 21 + i;
            tris[i * 6 + 3] = i;
            tris[i * 6 + 4] = i == 19 ? 20 : 21 + i;
            tris[i * 6 + 5] = i == 19 ? 0 : i + 1;
        }
        Mesh internode = new Mesh();
        internode.vertices = verts;
        internode.triangles = tris;
        return internode;
    }

    public Mesh assembleMeshes() {
        Mesh branch = new Mesh();
        CombineInstance[] segments = new CombineInstance[internodeMeshes.Count + 1];
        for (int i = 0; i < internodeMeshes.Count; i++) {
            segments[i].mesh = internodeMeshes[i];
        }
        Vector3[] verts = new Vector3[21];
        for (int i = 0; i < 20; i++) {
            verts[i] = currTrunkCenter + branchRadius * (norm * Mathf.Cos(Mathf.Deg2Rad * i * 18) + binorm * Mathf.Sin(Mathf.Deg2Rad * i * 18));
        }
        verts[20] = currTrunkCenter + branchRadius * tan;
        int[] tris = new int[60];
        for (int i = 0; i < 20; i++) {
            tris[i * 3] = i;
            tris[i * 3 + 1] = 20;
            tris[i * 3 + 2] = i + 1;
        }
        Mesh branchCap = new Mesh();
        branchCap.vertices = verts;
        branchCap.triangles = tris;
        segments[internodeMeshes.Count].mesh = branchCap;
        branch.CombineMeshes(segments, true, false);
        return branch;
    }
    
    public Mesh assembleLeaves() {
        Mesh leaves = new Mesh();
        CombineInstance[] segments = new CombineInstance[leafMeshes.Count];
        for (int i = 0; i < leafMeshes.Count; i++) {
            segments[i].mesh = leafMeshes[i];
        }
        leaves.CombineMeshes(segments, true, false);
        return leaves;
    }
    
    public Color getBarkColor() {
        return barkColor;
    }
}