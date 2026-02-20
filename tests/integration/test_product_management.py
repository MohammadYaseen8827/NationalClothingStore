"""
Integration tests for Product Management Workflow
Tests the complete product catalog management business workflow
"""

import pytest
import requests
import json
from typing import Dict, Any, List
from datetime import datetime, timedelta

class TestProductManagementWorkflow:
    """Integration tests for complete product management workflow"""
    
    def setup_method(self):
        """Setup test environment"""
        self.base_url = "http://localhost:5000/api"
        self.auth_headers = self._get_auth_headers()
        self.cleanup_data = []
        
    def teardown_method(self):
        """Cleanup test data"""
        # Clean up created test data
        for item in self.cleanup_data:
            try:
                if item["type"] == "product":
                    requests.delete(
                        f"{self.base_url}/products/{item['id']}",
                        headers=self.auth_headers
                    )
                elif item["type"] == "category":
                    requests.delete(
                        f"{self.base_url}/categories/{item['id']}",
                        headers=self.auth_headers
                    )
            except:
                pass  # Ignore cleanup errors
        
    def _get_auth_headers(self) -> Dict[str, str]:
        """Get authentication headers for testing"""
        return {
            "Authorization": "Bearer test_token",
            "Content-Type": "application/json"
        }
    
    def test_complete_product_catalog_workflow(self):
        """Test complete product catalog creation and management workflow"""
        
        # Step 1: Create a parent category
        parent_category_data = {
            "name": "Clothing",
            "description": "Main clothing category",
            "isActive": True,
            "parentId": None
        }
        
        parent_response = requests.post(
            f"{self.base_url}/categories",
            json=parent_category_data,
            headers=self.auth_headers
        )
        
        assert parent_response.status_code == 201
        parent_category = parent_response.json()
        self.cleanup_data.append({"type": "category", "id": parent_category["id"]})
        
        # Step 2: Create a subcategory
        subcategory_data = {
            "name": "Men's Shirts",
            "description": "Men's casual and formal shirts",
            "isActive": True,
            "parentId": parent_category["id"]
        }
        
        subcategory_response = requests.post(
            f"{self.base_url}/categories",
            json=subcategory_data,
            headers=self.auth_headers
        )
        
        assert subcategory_response.status_code == 201
        subcategory = subcategory_response.json()
        self.cleanup_data.append({"type": "category", "id": subcategory["id"]})
        
        # Step 3: Create a product
        product_data = {
            "name": "Classic Cotton Shirt",
            "description": "Comfortable cotton shirt for everyday wear",
            "sku": "SHIRT-001",
            "categoryId": subcategory["id"],
            "isActive": True,
            "basePrice": 49.99,
            "costPrice": 25.00
        }
        
        product_response = requests.post(
            f"{self.base_url}/products",
            json=product_data,
            headers=self.auth_headers
        )
        
        assert product_response.status_code == 201
        product = product_response.json()
        self.cleanup_data.append({"type": "product", "id": product["id"]})
        
        # Step 4: Create product variations (different sizes and colors)
        variations = [
            {
                "productId": product["id"],
                "size": "S",
                "color": "White",
                "sku": "SHIRT-001-S-WHITE",
                "isActive": True,
                "additionalPrice": 0.00,
                "stockQuantity": 50
            },
            {
                "productId": product["id"],
                "size": "M",
                "color": "White",
                "sku": "SHIRT-001-M-WHITE",
                "isActive": True,
                "additionalPrice": 0.00,
                "stockQuantity": 75
            },
            {
                "productId": product["id"],
                "size": "L",
                "color": "Blue",
                "sku": "SHIRT-001-L-BLUE",
                "isActive": True,
                "additionalPrice": 5.00,
                "stockQuantity": 30
            }
        ]
        
        created_variations = []
        for variation_data in variations:
            variation_response = requests.post(
                f"{self.base_url}/products/variations",
                json=variation_data,
                headers=self.auth_headers
            )
            
            assert variation_response.status_code == 201
            variation = variation_response.json()
            created_variations.append(variation)
        
        # Step 5: Verify product with variations
        product_with_variations_response = requests.get(
            f"{self.base_url}/products/{product['id']}",
            headers=self.auth_headers
        )
        
        assert product_with_variations_response.status_code == 200
        product_with_variations = product_with_variations_response.json()
        
        # Verify product details
        assert product_with_variations["name"] == product_data["name"]
        assert product_with_variations["sku"] == product_data["sku"]
        assert product_with_variations["categoryId"] == subcategory["id"]
        assert product_with_variations["basePrice"] == product_data["basePrice"]
        
        # Step 6: Update product information
        update_data = {
            "name": "Premium Cotton Shirt",
            "description": "Premium quality cotton shirt with enhanced comfort",
            "basePrice": 59.99,
            "isActive": True
        }
        
        update_response = requests.put(
            f"{self.base_url}/products/{product['id']}",
            json=update_data,
            headers=self.auth_headers
        )
        
        assert update_response.status_code == 200
        updated_product = update_response.json()
        
        # Verify updates
        assert updated_product["name"] == update_data["name"]
        assert updated_product["description"] == update_data["description"]
        assert updated_product["basePrice"] == update_data["basePrice"]
        
        # Step 7: Verify variations are still intact after product update
        variations_response = requests.get(
            f"{self.base_url}/products/{product['id']}/variations",
            headers=self.auth_headers
        )
        
        assert variations_response.status_code == 200
        variations_data = variations_response.json()
        
        assert len(variations_data["variations"]) == len(variations)
        assert variations_data["totalCount"] == len(variations)
        
        # Step 8: Test category hierarchy retrieval
        categories_response = requests.get(
            f"{self.base_url}/categories",
            headers=self.auth_headers
        )
        
        assert categories_response.status_code == 200
        categories_data = categories_response.json()
        
        # Verify parent-child relationship
        parent_found = False
        child_found = False
        
        for category in categories_data["categories"]:
            if category["id"] == parent_category["id"]:
                parent_found = True
            elif category["id"] == subcategory["id"]:
                child_found = True
                assert category["parentId"] == parent_category["id"]
        
        assert parent_found and child_found
        
        # Step 9: Test product search and filtering
        search_response = requests.get(
            f"{self.base_url}/products?search=Cotton&categoryId={subcategory['id']}",
            headers=self.auth_headers
        )
        
        assert search_response.status_code == 200
        search_data = search_response.json()
        
        # Should find our product
        assert search_data["totalCount"] >= 1
        product_found = False
        for product_item in search_data["products"]:
            if product_item["id"] == product["id"]:
                product_found = True
                break
        
        assert product_found
        
        # Step 10: Test inventory management (if available)
        # This would test stock updates, low stock alerts, etc.
        
        # Step 11: Test product deactivation (soft delete)
        deactivate_response = requests.put(
            f"{self.base_url}/products/{product['id']}",
            json={"isActive": False},
            headers=self.auth_headers
        )
        
        assert deactivate_response.status_code == 200
        deactivated_product = deactivate_response.json()
        
        assert deactivated_product["isActive"] == False
        
        # Step 12: Verify deactivated product doesn't appear in active searches
        active_search_response = requests.get(
            f"{self.base_url}/products?isActive=true",
            headers=self.auth_headers
        )
        
        assert active_search_response.status_code == 200
        active_search_data = active_search_response.json()
        
        # Product should not appear in active results
        product_not_found = True
        for product_item in active_search_data["products"]:
            if product_item["id"] == product["id"]:
                product_not_found = False
                break
        
        assert product_not_found
        
        # Step 13: Verify product still appears in all products search
        all_products_response = requests.get(
            f"{self.base_url}/products?includeInactive=true",
            headers=self.auth_headers
        )
        
        assert all_products_response.status_code == 200
        all_products_data = all_products_response.json()
        
        # Product should appear in all results
        product_found_in_all = False
        for product_item in all_products_data["products"]:
            if product_item["id"] == product["id"]:
                product_found_in_all = True
                assert product_item["isActive"] == False
                break
        
        assert product_found_in_all
    
    def test_category_hierarchy_workflow(self):
        """Test category hierarchy management workflow"""
        
        # Create nested category structure
        categories = [
            {"name": "Apparel", "description": "All clothing items", "parentId": None},
            {"name": "Tops", "description": "Upper body clothing", "parentId": None},
            {"name": "Bottoms", "description": "Lower body clothing", "parentId": None}
        ]
        
        created_categories = []
        
        # Create parent categories
        for category_data in categories:
            response = requests.post(
                f"{self.base_url}/categories",
                json=category_data,
                headers=self.auth_headers
            )
            
            assert response.status_code == 201
            category = response.json()
            created_categories.append(category)
            self.cleanup_data.append({"type": "category", "id": category["id"]})
        
        # Create subcategories
        subcategories = [
            {"name": "T-Shirts", "description": "Casual t-shirts", "parentId": created_categories[1]["id"]},
            {"name": "Dress Shirts", "description": "Formal shirts", "parentId": created_categories[1]["id"]},
            {"name": "Jeans", "description": "Denim jeans", "parentId": created_categories[2]["id"]},
            {"name": "Shorts", "description": "Casual shorts", "parentId": created_categories[2]["id"]}
        ]
        
        created_subcategories = []
        
        for subcategory_data in subcategories:
            response = requests.post(
                f"{self.base_url}/categories",
                json=subcategory_data,
                headers=self.auth_headers
            )
            
            assert response.status_code == 201
            subcategory = response.json()
            created_subcategories.append(subcategory)
            self.cleanup_data.append({"type": "category", "id": subcategory["id"]})
        
        # Verify hierarchy structure
        hierarchy_response = requests.get(
            f"{self.base_url}/categories?includeHierarchy=true",
            headers=self.auth_headers
        )
        
        assert hierarchy_response.status_code == 200
        hierarchy_data = hierarchy_response.json()
        
        # Verify parent-child relationships
        for subcategory in created_subcategories:
            found = False
            for category in hierarchy_data["categories"]:
                if category["id"] == subcategory["id"]:
                    found = True
                    assert category["parentId"] in [cat["id"] for cat in created_categories]
                    break
            assert found
    
    def test_product_variation_management_workflow(self):
        """Test product variation management workflow"""
        
        # Create a test category first
        category_response = requests.post(
            f"{self.base_url}/categories",
            json={"name": "Test Category", "description": "Test", "isActive": True},
            headers=self.auth_headers
        )
        
        assert category_response.status_code == 201
        category = category_response.json()
        self.cleanup_data.append({"type": "category", "id": category["id"]})
        
        # Create a test product
        product_response = requests.post(
            f"{self.base_url}/products",
            json={
                "name": "Test Product",
                "description": "Test product",
                "sku": "TEST-001",
                "categoryId": category["id"],
                "isActive": True,
                "basePrice": 29.99,
                "costPrice": 15.00
            },
            headers=self.auth_headers
        )
        
        assert product_response.status_code == 201
        product = product_response.json()
        self.cleanup_data.append({"type": "product", "id": product["id"]})
        
        # Create multiple variations
        sizes = ["XS", "S", "M", "L", "XL"]
        colors = ["Red", "Blue", "Green", "Black"]
        
        created_variations = []
        
        for size in sizes:
            for color in colors:
                variation_data = {
                    "productId": product["id"],
                    "size": size,
                    "color": color,
                    "sku": f"TEST-001-{size}-{color}",
                    "isActive": True,
                    "additionalPrice": 0.00,
                    "stockQuantity": 100
                }
                
                response = requests.post(
                    f"{self.base_url}/products/variations",
                    json=variation_data,
                    headers=self.auth_headers
                )
                
                assert response.status_code == 201
                variation = response.json()
                created_variations.append(variation)
        
        # Verify all variations were created
        variations_response = requests.get(
            f"{self.base_url}/products/{product['id']}/variations",
            headers=self.auth_headers
        )
        
        assert variations_response.status_code == 200
        variations_data = variations_response.json()
        
        assert variations_data["totalCount"] == len(sizes) * len(colors)
        assert len(variations_data["variations"]) == len(created_variations)
        
        # Test variation filtering
        filtered_response = requests.get(
            f"{self.base_url}/products/{product['id']}/variations?size=M&color=Blue",
            headers=self.auth_headers
        )
        
        assert filtered_response.status_code == 200
        filtered_data = filtered_response.json()
        
        assert filtered_data["totalCount"] == 1
        assert filtered_data["variations"][0]["size"] == "M"
        assert filtered_data["variations"][0]["color"] == "Blue"
        
        # Test variation update
        update_variation = created_variations[0]
        update_data = {
            "additionalPrice": 5.00,
            "stockQuantity": 150,
            "isActive": True
        }
        
        update_response = requests.put(
            f"{self.base_url}/products/variations/{update_variation['id']}",
            json=update_data,
            headers=self.auth_headers
        )
        
        assert update_response.status_code == 200
        updated_variation = update_response.json()
        
        assert updated_variation["additionalPrice"] == 5.00
        assert updated_variation["stockQuantity"] == 150
    
    def test_error_handling_and_validation_workflow(self):
        """Test error handling and validation in product management workflow"""
        
        # Test duplicate SKU creation
        category_response = requests.post(
            f"{self.base_url}/categories",
            json={"name": "Test Category", "description": "Test", "isActive": True},
            headers=self.auth_headers
        )
        
        assert category_response.status_code == 201
        category = category_response.json()
        self.cleanup_data.append({"type": "category", "id": category["id"]})
        
        # Create first product
        product_data = {
            "name": "Test Product",
            "description": "Test product",
            "sku": "DUPLICATE-SKU-TEST",
            "categoryId": category["id"],
            "isActive": True,
            "basePrice": 29.99,
            "costPrice": 15.00
        }
        
        first_response = requests.post(
            f"{self.base_url}/products",
            json=product_data,
            headers=self.auth_headers
        )
        
        assert first_response.status_code == 201
        first_product = first_response.json()
        self.cleanup_data.append({"type": "product", "id": first_product["id"]})
        
        # Try to create second product with same SKU
        second_response = requests.post(
            f"{self.base_url}/products",
            json=product_data,
            headers=self.auth_headers
        )
        
        assert second_response.status_code == 400
        error_data = second_response.json()
        assert "errors" in error_data or "message" in error_data
        
        # Test invalid category ID
        invalid_product_data = {
            "name": "Invalid Product",
            "description": "Invalid product",
            "sku": "INVALID-SKU",
            "categoryId": "invalid-category-id",
            "isActive": True,
            "basePrice": 29.99,
            "costPrice": 15.00
        }
        
        invalid_response = requests.post(
            f"{self.base_url}/products",
            json=invalid_product_data,
            headers=self.auth_headers
        )
        
        assert invalid_response.status_code == 400 or invalid_response.status_code == 404
        
        # Test validation errors
        validation_errors_data = {
            "name": "",  # Empty name
            "sku": "",   # Empty SKU
            "categoryId": category["id"],
            "basePrice": -10.00,  # Negative price
            "costPrice": -5.00    # Negative cost
        }
        
        validation_response = requests.post(
            f"{self.base_url}/products",
            json=validation_errors_data,
            headers=self.auth_headers
        )
        
        assert validation_response.status_code == 400

if __name__ == "__main__":
    pytest.main([__file__, "-v"])
