import csv
import os
import re
from pathlib import Path

path = f"{Path(__file__).parent}/AdvancedToolsUnityProject/Assets/Data"
folders = {"CombinedMesh_Cube_LowInterval", "GPUInstanced_Cube_LowInterval", "IndividualObjects_Cube_LowInterval"}

def ConvertDataFolder(root_folder, name):
    NewData = [["Object count", "Average FPS"]]

    path = f"{root_folder}/{name}"
    for filename in os.listdir(path):
        if filename.endswith(".meta"):
            continue

        file_path = os.path.join(path, filename)
        if os.path.isfile(file_path):
            NewData.append([re.findall(r'\d+', filename)[0], str(ConvertFileToAverageFPS(file_path))])

    with open(f'{root_folder}/Converted/{name}_Converted.csv', mode='w', newline='') as file:
        writer = csv.writer(file)
        writer.writerows(NewData)


def ConvertFileToAverageFPS(filepath):
    with open(filepath, mode='r', newline='') as file:
        reader = csv.reader(file)
        data = list(reader)
        return GetAverageFPS(data)

def GetAverageFPS(data):
    frames = -1
    time = 0
    # Example: print the data
    for row in data:
        frames += 1
        if frames == 0:
            continue
        time = float(row[1])

    return frames / time


### --------------- main --------------- ###

for f in folders:
    ConvertDataFolder(path, f)

