import os
import re
import json

frontend_root = r'd:\Work\Master projects\National Clothing Store\frontend\src'
services_path = os.path.join(frontend_root, 'services')
components_path = frontend_root

def get_files(path, extension):
    files = []
    for root, _, filenames in os.walk(path):
        for filename in filenames:
            if filename.endswith(extension):
                files.append(os.path.join(root, filename))
    return files

def scan_frontend_services():
    services = []
    service_files = get_files(services_path, '.ts')
    
    for file_path in service_files:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()
            service_name = os.path.basename(file_path).replace('.ts', '')
            
            methods = []
            seen = set()
            
            # Find all async methods with their bodies
            # Pattern: async methodName(...) { ... }
            # We need to find methods that contain API calls
            
            # Get all method definitions with their positions
            for method_match in re.finditer(r'async\s+(\w+)\s*\([^)]*\)\s*[:{]', content):
                method_name = method_match.group(1)
                method_start = method_match.start()
                
                # Find the end of this method (matching closing brace)
                brace_count = 0
                method_end = method_start
                in_method = False
                for i in range(method_start, len(content)):
                    if content[i] == '{':
                        brace_count += 1
                        in_method = True
                    elif content[i] == '}':
                        brace_count -= 1
                        if in_method and brace_count == 0:
                            method_end = i + 1
                            break
                
                method_body = content[method_start:method_end]
                
                # Look for API calls in this method body
                # Pattern 1: Direct: apiClient.get('/path')
                for m in re.finditer(r"apiClient\.(get|post|put|delete|patch)\s*(?:<[^>]*>)?\s*\(\s*['\"]([^'\"]+)['\"]", method_body):
                    verb = m.group(1)
                    url = m.group(2)
                    normalized = re.sub(r'\$\{(\w+)\}', r'{\1}', url)
                    key = (method_name, verb, normalized)
                    if key not in seen:
                        seen.add(key)
                        methods.append({
                            'name': method_name,
                            'verb': verb,
                            'url': normalized,
                            'params': ''
                        })
                
                # Pattern 2: Template: apiClient.get(`/path/${id}`)
                for m in re.finditer(r"apiClient\.(get|post|put|delete|patch)\s*(?:<[^>]*>)?\s*\(\s*`([^`]+)`", method_body):
                    verb = m.group(1)
                    url = m.group(2)
                    # Convert ${id} to {id} for matching
                    normalized = url.replace('$', '')
                    key = (method_name, verb, normalized)
                    if key not in seen:
                        seen.add(key)
                        methods.append({
                            'name': method_name,
                            'verb': verb,
                            'url': normalized,
                            'params': ''
                        })
            
            services.append({
                'name': service_name,
                'methods': methods
            })
    return services

def scan_component_usage():
    usage = {}
    src_files = get_files(frontend_root, ('.vue', '.ts'))
    for file_path in src_files:
        rel_path = os.path.relpath(file_path, frontend_root)
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()
            service_imports = re.findall(r'import\s+.*?\s+from\s+[\'"]@/services/(.*?)[\'"]', content)
            for si in service_imports:
                si_name = si.split('.')[0]
                if si_name not in usage: usage[si_name] = []
                usage[si_name].append(rel_path)
    return usage

def scan_frontend_types():
    types_dir = os.path.join(frontend_root, 'types')
    types = {}
    if os.path.exists(types_dir):
        for filename in os.listdir(types_dir):
            if filename.endswith('.ts'):
                types_path = os.path.join(types_dir, filename)
                with open(types_path, 'r', encoding='utf-8') as f:
                    content = f.read()
                    interface_matches = re.finditer(r'export\s+interface\s+(\w+).*?\{(.*?)\}', content, re.DOTALL)
                    for im in interface_matches:
                        name = im.group(1)
                        body = im.group(2)
                        props = re.findall(r'(\w+)\??:\s*(.*?)(?:\n|;)', body)
                        types[name] = {p[0]: p[1].strip() for p in props}
    return types

if __name__ == "__main__":
    data = {
        'services': scan_frontend_services(),
        'usage': scan_component_usage(),
        'types': scan_frontend_types()
    }
    with open('frontend_audit.json', 'w') as f:
        json.dump(data, f, indent=2)
    print("Frontend audit data saved to frontend_audit.json")
