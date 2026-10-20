"""Create original beveled warehouse utility-panel art in Blender; run with --background --python."""
import bpy, math
from pathlib import Path
root=Path(__file__).resolve().parents[2]
bpy.ops.wm.read_factory_settings(use_empty=True)
def mat(name,color,metal):
 m=bpy.data.materials.new(name);m.diffuse_color=(*color,1);m.use_nodes=True
 b=m.node_tree.nodes.get('Principled BSDF');b.inputs['Base Color'].default_value=(*color,1);b.inputs['Metallic'].default_value=metal;b.inputs['Roughness'].default_value=.35
 return m
steel=mat('Panel_Steel',(.16,.20,.22),.75);dark=mat('Panel_Recess',(.035,.045,.05),.55);amber=mat('Panel_Amber',(.75,.4,.05),.5);glass=mat('Panel_Green',(.1,.6,.4),.3)
def box(name,pos,scale,m,bevel=.01):
 bpy.ops.mesh.primitive_cube_add(size=1,location=pos);o=bpy.context.object;o.name=name;o.scale=scale;bpy.ops.object.transform_apply(location=False,rotation=False,scale=True);o.data.materials.append(m)
 if bevel:
  b=o.modifiers.new('Machined edges','BEVEL');b.width=bevel;b.segments=3
  bpy.context.view_layer.objects.active=o;bpy.ops.object.modifier_apply(modifier=b.name)
 for p in o.data.polygons:p.use_smooth=True
 n=o.modifiers.new('Weighted normals','WEIGHTED_NORMAL')
 return o
box('Enclosure',(0,0,0),(.9,.19,1.25),steel,.035)
box('Recessed_Door',(0,-.105,.02),(.79,.035,1.10),dark,.018)
box('Door_Plate',(0,-.13,.035),(.73,.025,1.04),steel,.02)
for x in [-.30,.30]:
 for z in [-.43,.43]:
  bpy.ops.mesh.primitive_cylinder_add(vertices=12,radius=.022,depth=.02,location=(x,-.16,z),rotation=(math.pi/2,0,0));bpy.context.object.name='Captive_Bolt';bpy.context.object.data.materials.append(amber)
for z in [-.31+i*.045 for i in range(8)]:box('Vent_Slat',(0,-.151,z),(.46,.012,.018),dark,.003)
box('Caution_Plaque',(-.1,-.154,.33),(.31,.012,.17),amber,.006)
for x in [.18,.26]:box('Status_Indicator',(x,-.16,.33),(.045,.025,.045),glass,.01)
box('Handle',(.28,-.19,-.02),(.035,.08,.17),dark,.009)
for z in [-.28,.28]:box('Hinge',(-.36,-.15,z),(.045,.07,.15),amber,.008)
for x in [-.25,.25]:
 box('Cable_Port',(x,.02,.67),(.1,.12,.11),dark,.01)
source=root/'Assets/Meshes/Source/IndustrialServicePanel.blend';dest=root/'Assets/Meshes/Props/IndustrialServicePanel.fbx'
bpy.ops.wm.save_as_mainfile(filepath=str(source))
bpy.ops.export_scene.fbx(filepath=str(dest),axis_forward='-Z',axis_up='Y',apply_unit_scale=True,use_mesh_modifiers=True,bake_anim=False)
print('Original Blender service panel saved and exported.')
