#load @"BindAsLegacyV2Runtime.fs"
// adjust these as needed for your latest installed version of ManagedDirectX
#I @"C:\WINDOWS\Microsoft.NET\DirectX for Managed Code\1.0.2902.0" 
#I @"C:\WINDOWS\Microsoft.NET\DirectX for Managed Code\1.0.2903.0"  
#I @"C:\WINDOWS\Microsoft.NET\DirectX for Managed Code\1.0.2904.0"  
#I @"C:\WINDOWS\Microsoft.NET\DirectX for Managed Code\1.0.2907.0"  


#r @"Microsoft.DirectX.dll"
#r @"Microsoft.DirectX.Direct3D.dll" 
#r @"Microsoft.DirectX.Direct3Dx.dll" 

#load @"dxlib.fs"
 
open System
open System.Drawing
open System.Windows.Forms
open Microsoft.DirectX
open Microsoft.DirectX.Direct3D
open Microsoft.FSharp.Control.CommonExtensions
open Sample.DirectX
open Sample.DirectX.MathOps
open Sample.DirectX.VectorOps

let form = new SmoothForm(Visible = true, TopMost = true, 
                          Text = "F# surface plot",
                          ClientSize = Size(600,400),
                          FormBorderStyle=FormBorderStyle.FixedSingle)


let renderer = new DirectXRenderer(form)

renderer.DrawScene.Add(fun _ -> renderer.DrawCubeAxis())

renderer.DrawScene.Add(fun _ -> renderer.SetupLights())



let mutable view = 
   { YawPitchRoll   = Matrix.RotationYawPitchRoll(0.0f,0.0f,0.0f);
     Focus          = scale 0.5f (X1 + Y1 + Z1);
     Zoom           = 4.0 }

renderer.DrawScene.Add(fun _ -> renderer.SetView(view))


let mouseTrack = MouseTracker(form)

mouseTrack.Add(fun (a,b) -> 
    let view2 = 
        let dx = b.X - a.X
        let dy = b.Y - a.Y
        match b.Button, Form.ModifierKeys with 
        | MouseButtons.Left, Keys.Shift -> view.AdjustZoom(dx,dy)
        | MouseButtons.Left, _          -> view.AdjustYawPitchRoll(dx,dy)
        | _                             -> view.AdjustFocus(dx,dy)  
    view <- view2
)

let mutable ff    = (fun (t:float32) x y -> x * (1.0f - y))

/// Z-range
let mutable range = (0.0f,1.0f)

/// XY-mesh
let mutable mesh = BaseMesh.Grid(20,20)

//mesh

// Scale w.r.t. range ... 
let scalef (min,max) (z:float32) = (z-min) / (max-min) 

// Get the function and scale it 
let theFunction t x y = ff t x y |> scalef range

renderer.DrawScene.Add(fun t -> renderer.DrawSurface mesh (theFunction t))

//----------------------------------------------------------------------------
// PART 2 - change the function

ff <- (fun t x y -> sqr (x - 0.5f) * sqr (y - 0.5f) * 16.0f)
ff <- (fun t x y -> 0.5f * sin(x * 4.5f + t / 2.0f) * cos(y * 8.0f) * x + 0.5f)

range <- (-1.0f,1.0f)
range <- (0.0f,1.0f)

let ripple t x y =
   let x,y = x - 0.5f,y - 0.5f 
   let r = sqrt (x*x + y*y) 
   exp(-5.0f * r) * sin(6.0f * pi * r + t) + 0.5f

ff <- ripple

  
mesh <- BaseMesh.Grid (50,50)

mesh <- BaseMesh.Grid (20,20)


let surfacePoint f x y = Vector3(x,y,f x y)

let surfaceNormal f x y =
    let dx,dy = 0.01f,0.01f 
    let pA    = surfacePoint f x y 
    let pA_dx = surfacePoint f (x+dx) y - pA 
    let pA_dy = surfacePoint f x (y+dy) - pA 
    normalize (cross pA_dx pA_dy)

let gravity = Vector3(0.0f,0.0f,-9.81f)

// A ball is a pair of position/velocity vectors
type ball = Ball of Vector3 * Vector3 

let radiusA = 0.010f 
let radiusB = 0.005f     

let moveBall f timeDelta (Ball (position,velocity)) =

   
    let nHat     = surfaceNormal f position.X position.Y  

    let acc      = planeProject nHat gravity              // acceleration in plane 
    let velocity = planeProject nHat velocity             // velocity     in plane 
    
    // Compute the new position
    let position = position + Vector3.Scale(velocity,timeDelta)  // iterate 
    let velocity = velocity + Vector3.Scale(acc     ,timeDelta)  // iterate 

    // Handle the bounce!
    let bounce (p,v) =                                        
        if   (p < 0.0f + radiusA) then (2.0f * (0.0f + radiusA) - p,-v) 
        elif (p > 1.0f - radiusA) then (2.0f * (1.0f - radiusA) - p,-v) 
        else                           (p,v)  
    let px,vx = bounce (position.X,velocity.X)              // bounce X edges 
    let py,vy = bounce (position.Y,velocity.Y)              // bounce Y edges 
    let position = surfacePoint f px py                     // keep to surface 
    let velocity = Vector3 (vx,vy,velocity.Z) 
    let velocity = planeProject nHat velocity               // velocity in plane     

    Ball (position,velocity)



let drawBall t (Ball (p,v)) =
    let n    = surfaceNormal (theFunction t) p.X p.Y 
    // position XY-projection 
    let p0   = Vector3(p.X,p.Y,0.0f) 
    // unit velocity XY-projection 
    let pV   = Vector3(v.X,v.Y,0.0f)                  
    // and it's XY-perpendicular 
    let pVxZ = Vector3.Cross(pV,Z1)                         
    // vertical line 
    renderer.DrawLines (Array.map (Vertex.Colored Color.Gray) [| p0;p |])
    // velocity arrow on floor 
    renderer.DrawPlaneArrow Z1 p0 pV      
    // normal arrow at point     
    renderer.DrawPlaneArrow (cross n X1) p  (scale 0.8f n)
    renderer.Device.Transform.World <- 
        (let m = Matrix.LookAtLH(p + scale radiusB n,p+n,X1) 
         Matrix.Invert(m))
      
    // Now draw the mesh
    using (Mesh .Torus(renderer.Device,radiusB,radiusA,20,20)) (fun mesh -> 
        mesh.ComputeNormals()
        mesh.DrawSubset(0))
      
    renderer.Device.Transform.World <- Matrix.Identity

let mutable active = [] : ball list
let addBall ball = active <- (ball :: active)  
let drawBalls t =  active |> List.iter(drawBall t) 
let mutable timeDelta = 0.008f
let moveBalls t = 
         let active' = active |> List.map (moveBall (theFunction t) timeDelta)
         active <- active'

//timeDelta <- 0.014f
renderer.DrawScene.Add(fun t -> moveBalls t) 
renderer.DrawScene.Add(fun t -> drawBalls t)


let bowl t x y = 
   let f phi u = ((1.0f + cos(2.0f * pi * u + phi )) / 2.0f) 
   f t x * f 0.0f y + 1.0f

range <- (0.0f,2.0f)
ff    <- (fun t -> bowl 0.0f)


// Second, add a ball 
addBall (Ball (Vector3(0.1f,0.1f,0.1f),
               Vector3(0.6f,0.5f,0.0f)))


// Add a ball train. 

Async.Start
    (async { for i in 0 .. 6 do 
                do addBall (Ball (Vector3(0.1f,0.1f,0.1f),
                                  Vector3(0.6f,0.5f,0.0f)))
                do! Async.Sleep(100)  })

// Now move the floor!

let mutable rate = 0.25f 
ff <- (fun t x y -> bowl (rate * t) x y)
rate <- 1.0f
rate <- 2.0f

ff <- ripple
range <- (0.0f,1.0f)

mesh <- BaseMesh.Grid (30,30)

#if COMPILED
[]
do Application.Run(form)

do Application.Exit()
#endif
