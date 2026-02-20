"""
Contract tests for Product Catalog API endpoints
Tests the API contracts and business rules for product catalog management
"""

import pytest
import requests
import json
from typing import Dict, Any, List
from datetime import datetime, timedelta

class TestProductCatalogContract:
    """Contract tests for product catalog management endpoints"""
    
    def setup_method(self):
        """Setup test environment"""
        self.base_url = "http://localhost:5000/api"
        self.auth_headers = self._get_auth_headers()
        
    def _get_auth_headers(self) -> Dict[str, str]:
        """Get authentication headers for testing"""
        # This would typically use a test user with appropriate permissions
        return {
            "Authorization": "Bearer test_token",
            "Content-Type": "application/json"
        }
    
    def test_create_category_contract(self):
        """Test contract for creating a product category"""
        # Arrange
        category_data = {
            "name": "Test Category",
            "description": "Test category description",
            "isActive": True,
            "parentId": None
        }
        
        # Act
        response = requests.post(
            f"{self.base_url}/categories",
            json=category_data,
            headers=self.auth_headers
        )
        
        # Assert - Contract validation
        assert response.status_code in [201, 400, 401, 403]  # Expected status codes
        
        if response.status_code == 201:
            data = response.json()
            # Validate response structure
            assert "id" in data
            assert "name" in data
            assert "description" in data
            assert "isActive" in data
            assert "createdAt" in data
            assert "updatedAt" in data
            
            # Validate data types
            assert isinstance(data["id"], str)
            assert isinstance(data["name"], str)
            assert isinstance(data["description"], str)
            assert isinstance(data["isActive"], bool)
            assert data["name"] == category_data["name"]
            assert data["description"] == category_data["description"]
            assert data["isActive"] == category_data["isActive"]
    
    def test_get_categories_contract(self):
        """Test contract for retrieving product categories"""
        # Act
        response = requests.get(
            f"{self.base_url}/categories",
            headers=self.auth_headers
        )
        
        # Assert - Contract validation
        assert response.status_code in [200, 401, 403]
        
        if response.status_code == 200:
            data = response.json()
            # Validate response structure
            assert "categories" in data
            assert "totalCount" in data
            assert "pageNumber" in data
            assert "pageSize" in data
            
            # Validate data types
            assert isinstance(data["categories"], list)
            assert isinstance(data["totalCount"], int)
            assert isinstance(data["pageNumber"], int)
            assert isinstance(data["pageSize"], int)
            
            # Validate category structure if any exist
            if data["categories"]:
                category = data["categories"][0]
                assert "id" in category
                assert "name" in category
                assert "description" in category
                assert "isActive" in category
    
    def test_create_product_contract(self):
        """Test contract for creating a product"""
        # Arrange
        product_data = {
            "name": "Test Product",
            "description": "Test product description",
            "sku": "TEST-SKU-001",
            "categoryId": "test-category-id",
            "isActive": True,
            "basePrice": 29.99,
            "costPrice": 15.50
        }
        
        # Act
        response = requests.post(
            f"{self.base_url}/products",
            json=product_data,
            headers=self.auth_headers
        )
        
        # Assert - Contract validation
        assert response.status_code in [201, 400, 401, 403, 404]
        
        if response.status_code == 201:
            data = response.json()
            # Validate response structure
            assert "id" in data
            assert "name" in data
            assert "description" in data
            assert "sku" in data
            assert "categoryId" in data
            assert "isActive" in data
            assert "basePrice" in data
            assert "costPrice" in data
            assert "createdAt" in data
            assert "updatedAt" in data
            
            # Validate data types
            assert isinstance(data["id"], str)
            assert isinstance(data["name"], str)
            assert isinstance(data["description"], str)
            assert isinstance(data["sku"], str)
            assert isinstance(data["categoryId"], str)
            assert isinstance(data["isActive"], bool)
            assert isinstance(data["basePrice"], (int, float))
            assert isinstance(data["costPrice"], (int, float))
            
            # Validate data values
            assert data["name"] == product_data["name"]
            assert data["sku"] == product_data["sku"]
            assert data["basePrice"] == product_data["basePrice"]
    
    def test_get_products_contract(self):
        """Test contract for retrieving products"""
        # Act
        response = requests.get(
            f"{self.base_url}/products",
            headers=self.auth_headers
        )
        
        # Assert - Contract validation
        assert response.status_code in [200, 401, 403]
        
        if response.status_code == 200:
            data = response.json()
            # Validate response structure
            assert "products" in data
            assert "totalCount" in data
            assert "pageNumber" in data
            assert "pageSize" in data
            
            # Validate data types
            assert isinstance(data["products"], list)
            assert isinstance(data["totalCount"], int)
            assert isinstance(data["pageNumber"], int)
            assert isinstance(data["pageSize"], int)
            
            # Validate product structure if any exist
            if data["products"]:
                product = data["products"][0]
                assert "id" in product
                assert "name" in product
                assert "sku" in product
                assert "categoryId" in product
                assert "basePrice" in product
                assert "isActive" in product
    
    def test_create_product_variation_contract(self):
        """Test contract for creating product variations"""
        # Arrange
        variation_data = {
            "productId": "test-product-id",
            "size": "M",
            "color": "Blue",
            "sku": "TEST-SKU-001-M-BLUE",
            "isActive": True,
            "additionalPrice": 0.00,
            "stockQuantity": 100
        }
        
        # Act
        response = requests.post(
            f"{self.base_url}/products/variations",
            json=variation_data,
            headers=self.auth_headers
        )
        
        # Assert - Contract validation
        assert response.status_code in [201, 400, 401, 403, 404]
        
        if response.status_code == 201:
            data = response.json()
            # Validate response structure
            assert "id" in data
            assert "productId" in data
            assert "size" in data
            assert "color" in data
            assert "sku" in data
            assert "isActive" in data
            assert "additionalPrice" in data
            assert "stockQuantity" in data
            assert "createdAt" in data
            assert "updatedAt" in data
            
            # Validate data types
            assert isinstance(data["id"], str)
            assert isinstance(data["productId"], str)
            assert isinstance(data["size"], str)
            assert isinstance(data["color"], str)
            assert isinstance(data["sku"], str)
            assert isinstance(data["isActive"], bool)
            assert isinstance(data["additionalPrice"], (int, float))
            assert isinstance(data["stockQuantity"], int)
    
    def test_get_product_variations_contract(self):
        """Test contract for retrieving product variations"""
        # Arrange
        product_id = "test-product-id"
        
        # Act
        response = requests.get(
            f"{self.base_url}/products/{product_id}/variations",
            headers=self.auth_headers
        )
        
        # Assert - Contract validation
        assert response.status_code in [200, 401, 403, 404]
        
        if response.status_code == 200:
            data = response.json()
            # Validate response structure
            assert "variations" in data
            assert "totalCount" in data
            
            # Validate data types
            assert isinstance(data["variations"], list)
            assert isinstance(data["totalCount"], int)
            
            # Validate variation structure if any exist
            if data["variations"]:
                variation = data["variations"][0]
                assert "id" in variation
                assert "productId" in variation
                assert "size" in variation
                assert "color" in variation
                assert "sku" in variation
                assert "stockQuantity" in variation
    
    def test_update_product_contract(self):
        """Test contract for updating a product"""
        # Arrange
        product_id = "test-product-id"
        update_data = {
            "name": "Updated Product Name",
            "description": "Updated description",
            "basePrice": 39.99,
            "isActive": True
        }
        
        # Act
        response = requests.put(
            f"{self.base_url}/products/{product_id}",
            json=update_data,
            headers=self.auth_headers
        )
        
        # Assert - Contract validation
        assert response.status_code in [200, 400, 401, 403, 404]
        
        if response.status_code == 200:
            data = response.json()
            # Validate response structure
            assert "id" in data
            assert "name" in data
            assert "description" in data
            assert "basePrice" in data
            assert "updatedAt" in data
            
            # Validate data was updated
            assert data["name"] == update_data["name"]
            assert data["description"] == update_data["description"]
            assert data["basePrice"] == update_data["basePrice"]
    
    def test_delete_product_contract(self):
        """Test contract for deleting a product"""
        # Arrange
        product_id = "test-product-id"
        
        # Act
        response = requests.delete(
            f"{self.base_url}/products/{product_id}",
            headers=self.auth_headers
        )
        
        # Assert - Contract validation
        assert response.status_code in [204, 400, 401, 403, 404]
        
        # 204 No Content should have empty response
        if response.status_code == 204:
            assert response.content == b""
    
    def test_error_response_contract(self):
        """Test contract for error responses"""
        # Test invalid request
        response = requests.post(
            f"{self.base_url}/products",
            json={"invalid": "data"},
            headers=self.auth_headers
        )
        
        # Assert error response structure
        assert response.status_code in [400, 401, 403]
        
        if response.status_code == 400:
            data = response.json()
            assert "errors" in data or "message" in data
            assert isinstance(data.get("errors", []), list)
    
    def test_pagination_contract(self):
        """Test contract for paginated responses"""
        # Act
        response = requests.get(
            f"{self.base_url}/products?pageNumber=1&pageSize=10",
            headers=self.auth_headers
        )
        
        # Assert - Contract validation
        assert response.status_code in [200, 401, 403]
        
        if response.status_code == 200:
            data = response.json()
            # Validate pagination structure
            assert "pageNumber" in data
            assert "pageSize" in data
            assert "totalCount" in data
            assert "totalPages" in data
            assert "hasNextPage" in data
            assert "hasPreviousPage" in data
            
            # Validate pagination logic
            assert data["pageNumber"] == 1
            assert data["pageSize"] == 10
            assert isinstance(data["totalPages"], int)
            assert isinstance(data["hasNextPage"], bool)
            assert isinstance(data["hasPreviousPage"], bool)

if __name__ == "__main__":
    pytest.main([__file__, "-v"])
