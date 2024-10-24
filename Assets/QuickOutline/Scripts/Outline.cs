//
//  Outline.cs
//  QuickOutline
//
//  Created by Chris Nolet on 3/30/18.
//  Copyright © 2018 Chris Nolet. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph;
using UnityEngine;

[DisallowMultipleComponent]

public class Outline : MonoBehaviour
{
  private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();

  public enum Mode
  {
    GrabObject,
    InteractObject,
    CraftingZone,
  }

  public Mode OutlineMode
  {
    get { return outlineMode; }
    set
    {
      outlineMode = value;
      needsUpdate = true;
    }
  }

  [Serializable]
  private class ListVector3
  {
    public List<Vector3> data;
  }

  [SerializeField]
  private Mode outlineMode;

  [Header("Optional")]

  [SerializeField, Tooltip("Precompute enabled: Per-vertex calculations are performed in the editor and serialized with the object. "
  + "Precompute disabled: Per-vertex calculations are performed at runtime in Awake(). This may cause a pause for large meshes.")]
  private bool precomputeOutline;

  [SerializeField, HideInInspector]
  private List<Mesh> bakeKeys = new List<Mesh>();

  [SerializeField, HideInInspector]
  private List<ListVector3> bakeValues = new List<ListVector3>();

  private Renderer[] renderers;
  private Material outlineMaskMaterial;
  private Material outlineFillMaterial;

  private bool needsUpdate;
  private Transform player;  // Référence au joueur
  private bool isPlayerInRange = false;

  void Awake()
  {

    // Cache renderers
    renderers = GetComponentsInChildren<Renderer>();

    // Instantiate outline materials
    outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));
    outlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));

    outlineMaskMaterial.name = "OutlineMask (Instance)";
    outlineFillMaterial.name = "OutlineFill (Instance)";

    // Retrieve or generate smooth normals
    LoadSmoothNormals();

    // Apply material properties immediately
    needsUpdate = true;
  }

  void OnEnable()
  {
    foreach (var renderer in renderers)
    {

      // Append outline shaders
      var materials = renderer.sharedMaterials.ToList();

      materials.Add(outlineMaskMaterial);
      materials.Add(outlineFillMaterial);

      renderer.materials = materials.ToArray();
    }
  }

  void OnValidate()
  {

    // Update material properties
    needsUpdate = true;

    // Clear cache when baking is disabled or corrupted
    if (!precomputeOutline && bakeKeys.Count != 0 || bakeKeys.Count != bakeValues.Count)
    {
      bakeKeys.Clear();
      bakeValues.Clear();
    }

    // Generate smooth normals when baking is enabled
    if (precomputeOutline && bakeKeys.Count == 0)
    {
      Bake();
    }
  }

  float detectionRange;  // Distance à laquelle le contour apparaît
  bool blinks;
  float blinkSpeed;
  float r;
  float g;
  float b;
  void Start()
  {
    player = GameObject.FindGameObjectWithTag("Player").transform;

    switch (outlineMode)
    {
      case Mode.GrabObject:
        detectionRange = 1f;
        blinks = true;
        blinkSpeed = 7f;
        r = 0;
        g = 0;
        b = 0;
        break;

      case Mode.InteractObject:
        detectionRange = 2f;
        blinks = true;
        blinkSpeed = 5f;
        r = 1;
        g = 1;
        b = 1;
        break;

      case Mode.CraftingZone:
        detectionRange = 10f;
        blinks = true;
        blinkSpeed = 1f;
        r = 0;
        g = 0;
        b = 0;
        break;
    }
  }

  void Update()
  {
    if (needsUpdate)
    {
      needsUpdate = false;

      outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
      outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
      outlineFillMaterial.SetFloat("_OutlineWidth", 2f);
    }

    float distanceToPlayer = Vector3.Distance(player.position, transform.position);
    isPlayerInRange = distanceToPlayer <= detectionRange;

    float alpha;
    if (isPlayerInRange)
    {
      alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
    }
    else {
      alpha = 0f;
    }
    if (!blinks) alpha = 1f;

    Color color = new Color(r, g, b, alpha);
    outlineFillMaterial.SetColor("_OutlineColor", color);
  }

  void OnDisable()
  {
    foreach (var renderer in renderers)
    {

      // Remove outline shaders
      var materials = renderer.sharedMaterials.ToList();

      materials.Remove(outlineMaskMaterial);
      materials.Remove(outlineFillMaterial);

      renderer.materials = materials.ToArray();
    }
  }

  void OnDestroy()
  {

    // Destroy material instances
    Destroy(outlineMaskMaterial);
    Destroy(outlineFillMaterial);
  }

  void Bake()
  {

    // Generate smooth normals for each mesh
    var bakedMeshes = new HashSet<Mesh>();

    foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
    {

      // Skip duplicates
      if (!bakedMeshes.Add(meshFilter.sharedMesh))
      {
        continue;
      }

      // Serialize smooth normals
      var smoothNormals = SmoothNormals(meshFilter.sharedMesh);

      bakeKeys.Add(meshFilter.sharedMesh);
      bakeValues.Add(new ListVector3() { data = smoothNormals });
    }
  }

  void LoadSmoothNormals()
  {

    // Retrieve or generate smooth normals
    foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
    {

      // Skip if smooth normals have already been adopted
      if (!registeredMeshes.Add(meshFilter.sharedMesh))
      {
        continue;
      }

      // Retrieve or generate smooth normals
      var index = bakeKeys.IndexOf(meshFilter.sharedMesh);
      var smoothNormals = (index >= 0) ? bakeValues[index].data : SmoothNormals(meshFilter.sharedMesh);

      // Store smooth normals in UV3
      meshFilter.sharedMesh.SetUVs(3, smoothNormals);

      // Combine submeshes
      var renderer = meshFilter.GetComponent<Renderer>();

      if (renderer != null)
      {
        CombineSubmeshes(meshFilter.sharedMesh, renderer.sharedMaterials);
      }
    }

    // Clear UV3 on skinned mesh renderers
    foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
    {

      // Skip if UV3 has already been reset
      if (!registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
      {
        continue;
      }

      // Clear UV3
      skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];

      // Combine submeshes
      CombineSubmeshes(skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer.sharedMaterials);
    }
  }

  List<Vector3> SmoothNormals(Mesh mesh)
  {

    // Group vertices by location
    var groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

    // Copy normals to a new list
    var smoothNormals = new List<Vector3>(mesh.normals);

    // Average normals for grouped vertices
    foreach (var group in groups)
    {

      // Skip single vertices
      if (group.Count() == 1)
      {
        continue;
      }

      // Calculate the average normal
      var smoothNormal = Vector3.zero;

      foreach (var pair in group)
      {
        smoothNormal += smoothNormals[pair.Value];
      }

      smoothNormal.Normalize();

      // Assign smooth normal to each vertex
      foreach (var pair in group)
      {
        smoothNormals[pair.Value] = smoothNormal;
      }
    }

    return smoothNormals;
  }

  void CombineSubmeshes(Mesh mesh, Material[] materials)
  {

    // Skip meshes with a single submesh
    if (mesh.subMeshCount == 1)
    {
      return;
    }

    // Skip if submesh count exceeds material count
    if (mesh.subMeshCount > materials.Length)
    {
      return;
    }

    // Append combined submesh
    mesh.subMeshCount++;
    mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
  }
}
