import re
import numpy as np
import matplotlib.pyplot as plt
import cv2
import os

def parse_txt_file(file_path):
    with open(file_path, 'r') as file:
        content = file.read()
    
    iterations = re.findall(r'iteration: (\d+)', content)
    particles_data = re.findall(r'particles: \[\[([-\d\s]+)\]\]', content)
    
    iterations = [int(it) for it in iterations]
    particles = [list(map(int, data.split())) for data in particles_data]
    
    return iterations, particles

def create_frames(iterations, particles, output_folder='frames'):
    if not os.path.exists(output_folder):
        os.makedirs(output_folder)
    
    for i, (iteration, particle_positions) in enumerate(zip(iterations, particles)):
        plt.figure(figsize=(10, 5))
        plt.scatter(particle_positions, [0] * len(particle_positions), c='b', s=50)
        plt.xlim(min(particle_positions) - 50, max(particle_positions) + 50)
        plt.ylim(-1, 1)
        plt.title(f'Iteration: {iteration}')
        plt.xlabel('Particle Position')
        plt.ylabel('Y-axis (always 0)')
        plt.grid(True)
        
        frame_path = os.path.join(output_folder, f'frame_{i:03d}.png')
        plt.savefig(frame_path)
        plt.close()
    
    return output_folder

def create_video(frames_folder, output_video='particles.mp4', fps=2):
    frame_files = sorted([f for f in os.listdir(frames_folder) if f.endswith('.png')])
    frame_path = os.path.join(frames_folder, frame_files[0])
    frame = cv2.imread(frame_path)
    h, w, _ = frame.shape
    
    fourcc = cv2.VideoWriter_fourcc(*'mp4v')
    video_writer = cv2.VideoWriter(output_video, fourcc, fps, (w, h))
    
    for frame_file in frame_files:
        frame_path = os.path.join(frames_folder, frame_file)
        frame = cv2.imread(frame_path)
        video_writer.write(frame)
    
    video_writer.release()
    print(f'Video saved as {output_video}')

if __name__ == "__main__":
    file_path = "C:/Users/chris\OneDrive - Politecnico di Milano/Politecnico di Milano/PhD - dottorato/GitHub repositories Lenovo/thesis_sole/evoluzione_particelle_pso_50.txt"  # Change this to your actual file path
    iterations, particles = parse_txt_file(file_path)
    frames_folder = create_frames(iterations, particles)
    create_video(frames_folder)
