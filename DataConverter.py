import csv
import os
import re
from pathlib import Path

path = f"{Path(__file__).parent}\\AdvancedToolsUnityProject\\Assets\\Data"
folders = {"V2_Sphere_Individual", "V2_Sphere_GPUInstanced", "V2_Sphere_Combined", 
           "V2_Cube_Individual", "V2_Cube_GPUInstanced", "V2_Cube_Combined",
           "V2_15K_Individual", "V2_15K_GPUInstanced", "V2_15K_Combined"}

def ConvertDataFolder(root_folder, name):
    NewData = [["Object count", "Average FPS", "Average RAM Usage"]]

    path = f"{root_folder}/{name}"
    for filename in os.listdir(path):
        if filename.endswith(".meta"):
            continue

        file_path = os.path.join(path, filename)
        if os.path.isfile(file_path):
            numbers = re.findall(r'\d+', filename)
            averageFPS, averageRamUsage = ConvertFile(file_path)
            NewData.append([numbers[len(numbers) - 1], str(averageFPS), str(averageRamUsage)])

    with open(f'{root_folder}\\Converted\\{name}_Converted.csv', mode='w', newline='') as file:
        writer = csv.writer(file)
        writer.writerows(NewData)


def ConvertFile(filepath):
    with open(filepath, mode='r', newline='') as file:
        reader = csv.reader(file)
        data = list(reader)
        return GetData(data)

def GetData(data):
    frames = -1
    time = 0
    ram = 0
    # Example: print the data
    for row in data:
        frames += 1
        if frames == 0:
            continue
        time = float(row[1])
        ram += float(row[4])

    return (frames / time, ram / frames)


### --------------- main --------------- ###

for f in folders:
    ConvertDataFolder(path, f)

